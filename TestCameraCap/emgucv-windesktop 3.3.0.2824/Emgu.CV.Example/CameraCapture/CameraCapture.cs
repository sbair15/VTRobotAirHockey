//----------------------------------------------------------------------------
//  Copyright (C) 2004-2017 by EMGU Corporation. All rights reserved.       
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;

namespace CameraCapture
{
   public partial class CameraCapture : Form
   {
      private VideoCapture _capture = null;
      private bool _captureInProgress;
      private Mat _frame;
      private Mat _grayFrame;
      private Mat _smallGrayFrame;
      private Mat _smoothedGrayFrame;
      private Mat _cannyFrame;

        private Mat _hsvFrame;
        private Mat _redDetectFrame;



        public CameraCapture()
      {
         InitializeComponent();
         CvInvoke.UseOpenCL = false;
         try
         {
            _capture = new VideoCapture();
            _capture.ImageGrabbed += ProcessFrame;
         }
         catch (NullReferenceException excpt)
         {
            MessageBox.Show(excpt.Message);
         }
         _frame = new Mat();
         _grayFrame = new Mat();
         _smallGrayFrame = new Mat();
         _smoothedGrayFrame = new Mat();
         _cannyFrame = new Mat();

         _hsvFrame = new Mat();
         _redDetectFrame = new Mat();
    }

      private void ProcessFrame(object sender, EventArgs arg)
      {
         if (_capture != null && _capture.Ptr != IntPtr.Zero)
         {
            _capture.Retrieve(_frame, 0);

            CvInvoke.CvtColor(_frame, _grayFrame, ColorConversion.Bgr2Gray);

            //CvInvoke.PyrDown(_grayFrame, _smallGrayFrame);

           // CvInvoke.PyrUp(_smallGrayFrame, _smoothedGrayFrame);

                //CvInvoke.Canny(_smoothedGrayFrame, _cannyFrame, 100, 60);


            #region Red Circle Detection
            //Makes a copy of the original frame for later when there is a need for it 
            _frame.CopyTo(_cannyFrame);
            //Comverts to from RGB to HSV values
            CvInvoke.CvtColor(_frame, _hsvFrame, ColorConversion.Bgr2Hsv);
            
            //Shows only "red" objects
            CvInvoke.InRange(_hsvFrame, new ScalarArray(new MCvScalar(0, 100, 100)), new ScalarArray(new MCvScalar(22, 255, 255)), _redDetectFrame);

            //CvInvoke.InRange(_hsvFrame, new ScalarArray(new MCvScalar(160, 100, 100)), new ScalarArray(new MCvScalar(179, 255, 255)), _redDetectFrameLower);
            //Combines the two ranges of red detection
            //CvInvoke.AddWeighted(_redDetectFrameLower, 1.0, _redDetectFrameUpper, 1.0, 0.0, _redDetectFrameComb);.
            //Circle Detect
            //CircleF[] circles = CvInvoke.HoughCircles(_redDetectFrame, HoughType.Gradient, 1.0, 20.0, _redDetectFrame.Rows/8, 100, 20);
            //CvInvoke.CvtColor(_redDetectFrame, _grayFrame, ColorConversion.Bgr2Gray);

            //Blurs the image to prevent false positives
            CvInvoke.GaussianBlur(_redDetectFrame, _redDetectFrame, new System.Drawing.Size(9,9), 2, 2);
            //Detects circles from the _redDetectFrame *Works much faster now due to the HSV scale shift
            CircleF[] circles = CvInvoke.HoughCircles(_redDetectFrame, HoughType.Gradient, 2.0, 20.0, 180.0, 120, 5);
            //If there are any circles
            if (circles.GetLength(0) != 0)
            {
                //Used to overlay the orginial image with detected circles
                //Iterate through all the circles ***(Only ever should have one for Air Hockey)***
                for (int index = 0; index < circles.GetLength(0); ++index)
                {
                    //Gets the center point of the circle, as a float
                    PointF center = circles[index].Center;
                    //Gets the radius of the circle as a float
                    float radius = circles[index].Radius;
                    //Writes a circle outline onto a copy of the original image
                    CvInvoke.Circle(_cannyFrame, Point.Round(center), (int)Math.Ceiling(radius), new MCvScalar(255, 255, 0), 10);
                    //Console.WriteLine("There is a circle");
                }
            }
                
            #endregion

            captureImageBox.Image = _redDetectFrame;
            grayscaleImageBox.Image = _grayFrame;
            smoothedGrayscaleImageBox.Image = _frame;
            cannyImageBox.Image = _cannyFrame;
            /*#region Added Circle Detection
            //Makes the program run very slowly
            double cannyThreshold = 180.0;
            double circleAccumulatorThreshold = 120;
            CircleF[] circles = CvInvoke.HoughCircles(_grayFrame, HoughType.Gradient, 2.0, 20.0, cannyThreshold, circleAccumulatorThreshold, 5);
            if (circles.GetLength(0) != 0)
            {
                Console.WriteLine("There is at least one detected circle");
            }


            #endregion*/

            }
        }

      private void captureButtonClick(object sender, EventArgs e)
      {
         if (_capture != null)
         {
            if (_captureInProgress)
            {  //stop the capture
               captureButton.Text = "Start Capture";
               _capture.Pause();
            }
            else
            {
               //start the capture
               captureButton.Text = "Stop";
               _capture.Start();
            }

            _captureInProgress = !_captureInProgress;
         }
      }

      private void ReleaseData()
      {
         if (_capture != null)
            _capture.Dispose();
      }

      private void FlipHorizontalButtonClick(object sender, EventArgs e)
      {
         if (_capture != null) _capture.FlipHorizontal = !_capture.FlipHorizontal;
      }

      private void FlipVerticalButtonClick(object sender, EventArgs e)
      {
         if (_capture != null) _capture.FlipVertical = !_capture.FlipVertical;
      }
   }
}
