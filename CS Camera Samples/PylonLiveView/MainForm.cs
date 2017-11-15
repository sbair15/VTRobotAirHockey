using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using PylonLiveView;
using Basler.Pylon;


namespace PylonLiveView
{
    // The main window.
    public partial class MainForm : Form
    {
        private Camera camera = null;
        private PixelDataConverter converter = new PixelDataConverter();
        private Stopwatch stopWatch = new Stopwatch();

        // Set up the controls and events to be used and update the device list.
        public MainForm()
        {
            InitializeComponent();

            // Set the default names for the controls.
            testImageControl.DefaultName = "Test Image Selector";
            pixelFormatControl.DefaultName = "Pixel Format";
            widthSliderControl.DefaultName = "Width";
            heightSliderControl.DefaultName = "Height";
            gainSliderControl.DefaultName = "Gain";
            exposureTimeSliderControl.DefaultName = "Exposure Time";

            // Update the list of available camera devices in the upper left area.
            UpdateDeviceList();

            // Disable all buttons.
            EnableButtons(false, false);
        }


        // Occurs when the single frame acquisition button is clicked.
        private void toolStripButtonOneShot_Click(object sender, EventArgs e)
        {
            OneShot(); // Start the grabbing of one image.
        }


        // Occurs when the continuous frame acquisition button is clicked.
        private void toolStripButtonContinuousShot_Click(object sender, EventArgs e)
        {
            ContinuousShot(); // Start the grabbing of images until grabbing is stopped.
        }


        // Occurs when the stop frame acquisition button is clicked.
        private void toolStripButtonStop_Click(object sender, EventArgs e)
        {
            Stop(); // Stop the grabbing of images.
        }


        // Occurs when a device with an opened connection is removed.
        private void OnConnectionLost(Object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper thread.
                BeginInvoke(new EventHandler<EventArgs>(OnConnectionLost), sender, e);
                return;
            }

            // Close the camera object.
            DestroyCamera();
            // Because one device is gone, the list needs to be updated.
            UpdateDeviceList();
        }


        // Occurs when the connection to a camera device is opened.
        private void OnCameraOpened(Object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper thread.
                BeginInvoke(new EventHandler<EventArgs>(OnCameraOpened), sender, e);
                return;
            }

            // The image provider is ready to grab. Enable the grab buttons.
            EnableButtons(true, false);
        }


        // Occurs when the connection to a camera device is closed.
        private void OnCameraClosed(Object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper thread.
                BeginInvoke(new EventHandler<EventArgs>(OnCameraClosed), sender, e);
                return;
            }

            // The camera connection is closed. Disable all buttons.
            EnableButtons(false, false);
        }


        // Occurs when a camera starts grabbing.
        private void OnGrabStarted(Object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper thread.
                BeginInvoke(new EventHandler<EventArgs>(OnGrabStarted), sender, e);
                return;
            }

            // Reset the stopwatch used to reduce the amount of displayed images. The camera may acquire images faster than the images can be displayed.

            stopWatch.Reset();

            // Do not update the device list while grabbing to reduce jitter. Jitter may occur because the GUI thread is blocked for a short time when enumerating.
            updateDeviceListTimer.Stop();

            // The camera is grabbing. Disable the grab buttons. Enable the stop button.
            EnableButtons(false, true);
        }


        // Occurs when an image has been acquired and is ready to be processed.
        private void OnImageGrabbed(Object sender, ImageGrabbedEventArgs e)
        {
            if (InvokeRequired)
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper GUI thread.
                // The grab result will be disposed after the event call. Clone the event arguments for marshaling to the GUI thread.
                BeginInvoke(new EventHandler<ImageGrabbedEventArgs>(OnImageGrabbed), sender, e.Clone());
                return;
            }

            try
            {
                // Acquire the image from the camera. Only show the latest image. The camera may acquire images faster than the images can be displayed.

                // Get the grab result.
                IGrabResult grabResult = e.GrabResult;

                // Check if the image can be displayed.
                if (grabResult.IsValid)
                {
                    // Reduce the number of displayed images to a reasonable amount if the camera is acquiring images very fast.
                    if (!stopWatch.IsRunning || stopWatch.ElapsedMilliseconds > 33)
                    {
                        stopWatch.Restart();

                        Bitmap bitmap = new Bitmap(grabResult.Width, grabResult.Height, PixelFormat.Format32bppRgb);
                        // Lock the bits of the bitmap.
                        BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                        // Place the pointer to the buffer of the bitmap.
                        converter.OutputPixelFormat = PixelType.BGRA8packed;
                        IntPtr ptrBmp = bmpData.Scan0;
                        converter.Convert(ptrBmp, bmpData.Stride * bitmap.Height, grabResult); //Exception handling TODO
                        bitmap.UnlockBits(bmpData);

                        // Assign a temporary variable to dispose the bitmap after assigning the new bitmap to the display control.
                        Bitmap bitmapOld = pictureBox.Image as Bitmap;
                        // Provide the display control with the new bitmap. This action automatically updates the display.
                        pictureBox.Image = bitmap;
                        if (bitmapOld != null)
                        {
                            // Dispose the bitmap.
                            bitmapOld.Dispose();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }
            finally
            {
                // Dispose the grab result if needed for returning it to the grab loop.
                e.DisposeGrabResultIfClone();
            }
        }


        // Occurs when a camera has stopped grabbing.
        private void OnGrabStopped(Object sender, GrabStopEventArgs e)
        {
            if (InvokeRequired)
            {
                // If called from a different thread, we must use the Invoke method to marshal the call to the proper thread.
                BeginInvoke(new EventHandler<GrabStopEventArgs>(OnGrabStopped), sender, e);
                return;
            }

            // Reset the stopwatch.
            stopWatch.Reset();

            // Re-enable the updating of the device list.
            updateDeviceListTimer.Start();

            // The camera stopped grabbing. Enable the grab buttons. Disable the stop button.
            EnableButtons(true, false);

            // If the grabbed stop due to an error, display the error message.
            if(e.Reason != GrabStopReason.UserRequest)
            {
                MessageBox.Show("A grab error occured:\n" + e.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Helps to set the states of all buttons.
        private void EnableButtons(bool canGrab, bool canStop)
        {
            toolStripButtonContinuousShot.Enabled = canGrab;
            toolStripButtonOneShot.Enabled = canGrab;
            toolStripButtonStop.Enabled = canStop;
        }


        // Stops the grabbing of images and handles exceptions.
        private void Stop()
        {
            // Stop the grabbing.
            try
            {
                camera.StreamGrabber.Stop();
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }
        }


        // Closes the camera object and handles exceptions.
        private void DestroyCamera()
        {
            // Disable all parameter controls.
            try
            {
                if (camera != null)
                {

                    testImageControl.Parameter = null;
                    pixelFormatControl.Parameter = null;
                    widthSliderControl.Parameter = null;
                    heightSliderControl.Parameter = null;
                    gainSliderControl.Parameter = null;
                    exposureTimeSliderControl.Parameter = null;
                }
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }

            // Destroy the camera object.
            try
            {
                if (camera != null)
                {
                    camera.Close();
                    camera.Dispose();
                    camera = null;
                }
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }
        }


        // Starts the grabbing of a single image and handles exceptions.
        private void OneShot()
        {
            try
            {
                // Starts the grabbing of one image.
                camera.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.SingleFrame);
                camera.StreamGrabber.Start(1, GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }
        }


        // Starts the continuous grabbing of images and handles exceptions.
        private void ContinuousShot()
        {
            try
            {
                // Start the grabbing of images until grabbing is stopped.
                camera.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.Continuous);
                camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }
        }


        // Updates the list of available camera devices.
        private void UpdateDeviceList()
        {
            try
            {
                // Ask the camera finder for a list of camera devices.
                List<ICameraInfo> allCameras = CameraFinder.Enumerate();

                ListView.ListViewItemCollection items = deviceListView.Items;

                // Loop over all cameras found.
                foreach (ICameraInfo cameraInfo in allCameras)
                {
                    // Loop over all cameras in the list of cameras.
                    bool newitem = true;
                    foreach (ListViewItem item in items)
                    {
                        ICameraInfo tag = item.Tag as ICameraInfo;

                        // Is the camera found already in the list of cameras?
                        if ( tag[CameraInfoKey.FullName] == cameraInfo[CameraInfoKey.FullName])
                        {
                            tag = cameraInfo;
                            newitem = false;
                            break;
                        }
                    }

                    // If the camera is not in the list, add it to the list.
                    if (newitem)
                    {
                        // Create the item to display.
                        ListViewItem item = new ListViewItem(cameraInfo[CameraInfoKey.FriendlyName]);

                        // Create the tool tip text.
                        string toolTipText = "";
                        foreach( KeyValuePair<string, string> kvp in cameraInfo)
                        {
                            toolTipText += kvp.Key + ": " + kvp.Value + "\n";
                        }
                        item.ToolTipText = toolTipText;

                        // Store the camera info in the displayed item.
                        item.Tag = cameraInfo;

                        // Attach the device data.
                        deviceListView.Items.Add(item);
                    }
                }



                // Remove old camera devices that have been disconnected.
                foreach (ListViewItem item in items)
                {
                    bool exists = false;

                    // For each camera in the list, check whether it can be found by enumeration.
                    foreach (ICameraInfo cameraInfo in allCameras)
                    {
                        if (((ICameraInfo)item.Tag)[CameraInfoKey.FullName] == cameraInfo[CameraInfoKey.FullName])
                        {
                            exists = true;
                            break;
                        }
                    }
                    // If the camera has not been found, remove it from the list view.
                    if (!exists)
                    {
                        deviceListView.Items.Remove(item);
                    }
                }
            }
            catch (Exception exception)
            {
                ShowException(exception);
            }
        }


        // Shows exceptions in a message box.
        private void ShowException(Exception exception)
        {
            MessageBox.Show("Exception caught:\n" + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        // Closes the camera object when the window is closed.
        private void MainForm_FormClosing(object sender, FormClosingEventArgs ev)
        {
            // Close the camera object.
            DestroyCamera();
        }


        // Occurs when a new camera has been selected in the list. Destroys the object of the currently opened camera device and
        // creates a new object for the selected camera device. After that, the connection to the selected camera device is opened.
        private void deviceListView_SelectedIndexChanged(object sender, EventArgs ev)
        {
            // Destroy the old camera object.
            if (camera != null)
            {
                DestroyCamera();
            }


            // Open the connection to the selected camera device.
            if (deviceListView.SelectedItems.Count > 0)
            {
                // Get the first selected item.
                ListViewItem item = deviceListView.SelectedItems[0];
                // Get the attached device data.
                ICameraInfo selectedCamera = item.Tag as ICameraInfo;
                try
                {
                    // Create a new camera object.
                    camera = new Camera (selectedCamera);

                    camera.CameraOpened += Configuration.AcquireContinuous;

                    // Register for the events of the image provider needed for proper operation.
                    camera.ConnectionLost += OnConnectionLost;
                    camera.CameraOpened += OnCameraOpened;
                    camera.CameraClosed += OnCameraClosed;
                    camera.StreamGrabber.GrabStarted += OnGrabStarted;
                    camera.StreamGrabber.ImageGrabbed += OnImageGrabbed;
                    camera.StreamGrabber.GrabStopped += OnGrabStopped;

                    // Open the connection to the camera device.
                    camera.Open();

                    // Set the parameter for the controls.
                    testImageControl.Parameter = camera.Parameters[PLCamera.TestImageSelector];
                    pixelFormatControl.Parameter = camera.Parameters[PLCamera.PixelFormat];
                    widthSliderControl.Parameter = camera.Parameters[PLCamera.Width];
                    heightSliderControl.Parameter = camera.Parameters[PLCamera.Height];
                    if (camera.Parameters.Contains(PLCamera.GainAbs))
                    {
                        gainSliderControl.Parameter = camera.Parameters[PLCamera.GainAbs];
                    }
                    else
                    {
                        gainSliderControl.Parameter = camera.Parameters[PLCamera.Gain];
                    }
                    if (camera.Parameters.Contains(PLCamera.ExposureTimeAbs))
                    {
                        exposureTimeSliderControl.Parameter = camera.Parameters[PLCamera.ExposureTimeAbs];
                    }
                    else
                    {
                        exposureTimeSliderControl.Parameter = camera.Parameters[PLCamera.ExposureTime];
                    }
                }
                catch (Exception exception)
                {
                    ShowException(exception);
                }
            }
        }


        // If the F5 key has been pressed, update the list of devices.
        private void deviceListView_KeyDown(object sender, KeyEventArgs ev)
        {
            if (ev.KeyCode == Keys.F5)
            {
                ev.Handled = true;
                // Update the list of available camera devices.
                UpdateDeviceList();
            }
        }


        // Timer callback used to periodically check whether displayed camera devices are still attached to the PC.
        private void updateDeviceListTimer_Tick(object sender, EventArgs e)
        {
            UpdateDeviceList();
        }
    }
}