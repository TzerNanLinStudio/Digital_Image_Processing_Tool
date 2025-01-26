#include "MyForm.h"

using namespace UI;

//全域物件
ClassOpenCV OpenCVObject;
//ClassSTCCamera STCCameraObject;

//全域變數
int SpecialNumber = 10;

//攝影機全域物件
CStApiAutoInit objStApiAutoInit;
CIStSystemPtr pIStSystem(CreateIStSystem());
CIStDevicePtrArray pIStDeviceList;
CIStDataStreamPtrArray pIStDataStreamList;

//攝影機全域變數
bool STCCameraErrorFlag = false;
int STCCameraReceivedFrame = -1;

//攝影機全域函式
void STCCameraInitial(void);

#pragma region UI
System::Void UI::MyForm::MainForm_Load(System::Object^ sender, System::EventArgs^ e)
{
	ShowWindow(::GetConsoleWindow(), SW_HIDE);

	STCCameraInitial();

	// 打印當前路徑
	// char buffer[FILENAME_MAX];
	// _getcwd(buffer, FILENAME_MAX);
	// std::cout << "Current path: " << buffer << std::endl;
}

System::Void UI::MyForm::MainForm_Closing(System::Object^  sender, System::Windows::Forms::FormClosingEventArgs ^ e)
{
	exit(0);
}

System::Void UI::MyForm::ToolStripMenuItem_UserMode_Click(System::Object^  sender, System::EventArgs^  e)
{
	ShowWindow(::GetConsoleWindow(), SW_HIDE);
	
	DeveloperModeFlag = false;
}

System::Void UI::MyForm::ToolStripMenuItem_DeveloperMode_Click(System::Object^  sender, System::EventArgs^  e)
{
	ShowWindow(::GetConsoleWindow(), SW_SHOW);
	
	DeveloperModeFlag = true;
}

System::Void UI::MyForm::button_Open_Click(System::Object^  sender, System::EventArgs^  e)
{
	System::String^ strfilename;
	OpenFileDialog ^ DialogOpenImage = gcnew OpenFileDialog();
	DialogOpenImage->Multiselect = false;

	try
	{
		if (DialogOpenImage->ShowDialog() == System::Windows::Forms::DialogResult::OK)
		{
			strfilename = DialogOpenImage->InitialDirectory + DialogOpenImage->FileName;
		}

		std::string std_strfilename = msclr::interop::marshal_as<std::string>(strfilename);

		OpenCVObject.OriginMat = imread(std_strfilename, CV_LOAD_IMAGE_UNCHANGED);
		
		if (OpenCVObject.OriginMat.channels() == 1)
		{
			cv::cvtColor(OpenCVObject.OriginMat, OpenCVObject.OriginMat, CV_GRAY2BGR);
		}

		OpenCVObject.PresentMat = OpenCVObject.OriginMat.clone();

		delete ShowCameraImage;
		ShowCameraImage = gcnew System::Drawing::Bitmap(OpenCVObject.PresentMat.cols, OpenCVObject.PresentMat.rows, OpenCVObject.PresentMat.step, System::Drawing::Imaging::PixelFormat::Format24bppRgb, System::IntPtr(OpenCVObject.PresentMat.data));
		this->pictureBox_ShowImage->Image = ShowCameraImage;
	}
	catch (System::Exception ^ ee)
	{
		MessageBox::Show("Error Message: " + ee->Message);
	}
}

System::Void UI::MyForm::button_Save_Click(System::Object^  sender, System::EventArgs^  e)
{
	SaveFileDialog	^ DialogSaveImage = gcnew SaveFileDialog();

	DialogSaveImage->Filter = ".bmp|*.bmp";

	try
	{
		delete ShowCameraImage;
		ShowCameraImage = gcnew System::Drawing::Bitmap(OpenCVObject.PresentMat.cols, OpenCVObject.PresentMat.rows, OpenCVObject.PresentMat.step, System::Drawing::Imaging::PixelFormat::Format24bppRgb, System::IntPtr(OpenCVObject.PresentMat.data));
		this->pictureBox_ShowImage->Image = ShowCameraImage;

		if (DialogSaveImage->ShowDialog() == System::Windows::Forms::DialogResult::OK)
		{
			ShowCameraImage->Save(DialogSaveImage->FileName);
		}
	}
	catch (System::Exception ^ ee)
	{
		MessageBox::Show("Error Occur!");
	}
}

System::Void UI::MyForm::button_Origin_Click(System::Object^  sender, System::EventArgs^  e)
{
	if (OpenCVObject.OriginMat.empty() == 0)
	{
		OpenCVObject.PresentMat.release();
		OpenCVObject.PresentMat = OpenCVObject.OriginMat.clone();

		delete ShowCameraImage;
		ShowCameraImage = gcnew System::Drawing::Bitmap(OpenCVObject.PresentMat.cols, OpenCVObject.PresentMat.rows, OpenCVObject.PresentMat.step, System::Drawing::Imaging::PixelFormat::Format24bppRgb, System::IntPtr(OpenCVObject.PresentMat.data));
		this->pictureBox_ShowImage->Image = ShowCameraImage;
	}
	else
	{
		MessageBox::Show("Error Occur!");
	}
}

System::Void UI::MyForm::button_Gray_Click(System::Object^  sender, System::EventArgs^  e)
{
	if (OpenCVObject.PresentMat.empty() == 0)
	{
		cvtColor(OpenCVObject.PresentMat, OpenCVObject.PresentMat, CV_BGR2GRAY);
		cvtColor(OpenCVObject.PresentMat, OpenCVObject.PresentMat, CV_GRAY2BGR);

		delete ShowCameraImage;
		ShowCameraImage = gcnew System::Drawing::Bitmap(OpenCVObject.PresentMat.cols, OpenCVObject.PresentMat.rows, OpenCVObject.PresentMat.step, System::Drawing::Imaging::PixelFormat::Format24bppRgb, System::IntPtr(OpenCVObject.PresentMat.data));
		this->pictureBox_ShowImage->Image = ShowCameraImage;
	}
	else
	{
		MessageBox::Show("Error Occur!");
	}
}

System::Void UI::MyForm::button_ApplyThreshold_Click(System::Object^  sender, System::EventArgs^  e)
{
	if (OpenCVObject.PresentMat.empty() == 0)
	{
		cvtColor(OpenCVObject.PresentMat, OpenCVObject.PresentMat, CV_BGR2GRAY);
		threshold(OpenCVObject.PresentMat, OpenCVObject.PresentMat, OpenCVObject.ThresholdValue, 255, THRESH_BINARY);
		cvtColor(OpenCVObject.PresentMat, OpenCVObject.PresentMat, CV_GRAY2BGR);

		delete ShowCameraImage;
		ShowCameraImage = gcnew System::Drawing::Bitmap(OpenCVObject.PresentMat.cols, OpenCVObject.PresentMat.rows, OpenCVObject.PresentMat.step, System::Drawing::Imaging::PixelFormat::Format24bppRgb, System::IntPtr(OpenCVObject.PresentMat.data));
		this->pictureBox_ShowImage->Image = ShowCameraImage;
	}
	else
	{
		MessageBox::Show("Error Occur!");
	}
}

System::Void UI::MyForm::button_ApplyHoughCircle_Click(System::Object^  sender, System::EventArgs^  e)
{
	if (OpenCVObject.PresentMat.empty() == 0)
	{
		Mat grayimage;
		vector<Vec3f> circles;

		cvtColor(OpenCVObject.PresentMat, grayimage, CV_BGR2GRAY);
		GaussianBlur(grayimage, grayimage, cv::Size(9, 9), 2, 2);
		HoughCircles(grayimage, circles, CV_HOUGH_GRADIENT, 2, grayimage.rows / OpenCVObject.HoughCircleDP, OpenCVObject.HoughCircleParameter002, OpenCVObject.HoughCircleParameter003, OpenCVObject.HoughCircleMinRadius, OpenCVObject.HoughCircleMaxRadius);

		for (size_t i = 0; i < circles.size(); i++)
		{
			cv::Point center(cvRound(circles[i][0]), cvRound(circles[i][1]));
			int radius = cvRound(circles[i][2]);

			circle(OpenCVObject.PresentMat, center, 3, Scalar(0, 255, 0), -1, 8, 0);//點
			circle(OpenCVObject.PresentMat, center, radius, Scalar(0, 0, 255), 3, 8, 0);//圓

			System::String^ StrX;
			System::String^ StrY;
			System::String^ StrRadius;
			char strX[10];
			char strY[10];
			char strRadius[10];
			
			itoa(center.x, strX, 10);
			StrX = gcnew System::String(strX);
			itoa(center.y, strY, 10);
			StrY = gcnew System::String(strY);
			itoa(radius, strRadius, 10);
			StrRadius = gcnew System::String(strRadius);

			this->richTextBox_GeneralLog->SelectedText = "Center : (" + StrX + "," + StrY + ") ; Radius : " + StrRadius + ".\n";
		}

		delete ShowCameraImage;
		ShowCameraImage = gcnew System::Drawing::Bitmap(OpenCVObject.PresentMat.cols, OpenCVObject.PresentMat.rows, OpenCVObject.PresentMat.step, System::Drawing::Imaging::PixelFormat::Format24bppRgb, System::IntPtr(OpenCVObject.PresentMat.data));
		this->pictureBox_ShowImage->Image = ShowCameraImage;
	}
	else
	{
		this->richTextBox_GeneralLog->SelectedText = "Error Occur!\n";
	}
}

System::Void UI::MyForm::button_ImageBuffer001_Register_Click(System::Object^  sender, System::EventArgs^  e)
{
	if (OpenCVObject.PresentMat.empty() == 0)
	{
		OpenCVObject.ImageBuffer[0].release();
		OpenCVObject.ImageBuffer[0] = OpenCVObject.PresentMat.clone();

		this->richTextBox_GeneralLog->SelectedText = "ImageBuffer(1) Registered Successfully!\n";
	}
	else
	{
		this->richTextBox_GeneralLog->SelectedText = "ImageBuffer(1) Registered Unsuccessfully!\n";
	}
}

System::Void UI::MyForm::button_ImageBuffer001_Show_Click(System::Object^  sender, System::EventArgs^  e)
{
	if (OpenCVObject.ImageBuffer[0].empty() == 0)
	{
		OpenCVObject.PresentMat.release();
		OpenCVObject.PresentMat = OpenCVObject.ImageBuffer[0].clone();

		delete ShowCameraImage;
		ShowCameraImage = gcnew System::Drawing::Bitmap(OpenCVObject.PresentMat.cols, OpenCVObject.PresentMat.rows, OpenCVObject.PresentMat.step, System::Drawing::Imaging::PixelFormat::Format24bppRgb, System::IntPtr(OpenCVObject.PresentMat.data));
		this->pictureBox_ShowImage->Image = ShowCameraImage;
	}
	else
	{
		MessageBox::Show("Error Occur!");
	}
}

System::Void UI::MyForm::button_ImageBuffer002_Register_Click(System::Object^  sender, System::EventArgs^  e)
{
	if (OpenCVObject.PresentMat.empty() == 0)
	{
		OpenCVObject.ImageBuffer[1].release();
		OpenCVObject.ImageBuffer[1] = OpenCVObject.PresentMat.clone();

		this->richTextBox_GeneralLog->SelectedText = "ImageBuffer(2) Registered Successfully!\n";
	}
	else
	{
		this->richTextBox_GeneralLog->SelectedText = "ImageBuffer(2) Registered Unsuccessfully!\n";
	}
}

System::Void UI::MyForm::button_ImageBuffer002_Show_Click(System::Object^  sender, System::EventArgs^  e)
{
	if (OpenCVObject.ImageBuffer[1].empty() == 0)
	{
		OpenCVObject.PresentMat.release();
		OpenCVObject.PresentMat = OpenCVObject.ImageBuffer[1].clone();

		delete ShowCameraImage;
		ShowCameraImage = gcnew System::Drawing::Bitmap(OpenCVObject.PresentMat.cols, OpenCVObject.PresentMat.rows, OpenCVObject.PresentMat.step, System::Drawing::Imaging::PixelFormat::Format24bppRgb, System::IntPtr(OpenCVObject.PresentMat.data));
		this->pictureBox_ShowImage->Image = ShowCameraImage;
	}
	else
	{
		MessageBox::Show("Error Occur!");
	}
}

System::Void UI::MyForm::timer_UpdateUI_Tick(System::Object^  sender, System::EventArgs^  e)
{
	System::String^ Str;
	char str[10];

	if (!STCCameraErrorFlag)
	{
		INodeMap *pINodeMap = pIStDeviceList[0]->GetRemoteIStPort()->GetINodeMap();

		GenApi::CNodePtr pINodeExposureTime(pINodeMap->GetNode("ExposureTime"));
		GenApi::CFloatPtr pIFloatExposureTime(pINodeExposureTime);
		double dblExposureTime = pIFloatExposureTime->GetValue();
		itoa(int(dblExposureTime), str, 10);
		Str = gcnew System::String(str);
		this->userControlOfCameraSetting_001->label_ExposureValue->Text = Str;

		GenApi::CNodePtr pINodeGain(pINodeMap->GetNode("Gain"));
		GenApi::CFloatPtr pIFloatGain(pINodeGain);
		double dblGain = pIFloatGain->GetValue();
		itoa(int(dblGain), str, 10);
		Str = gcnew System::String(str);
		this->userControlOfCameraSetting_001->label_GainValue->Text = Str;

		GenApi::CNodePtr pINodeFPS(pINodeMap->GetNode("AcquisitionFrameRate"));
		GenApi::CFloatPtr pIFloatFPS(pINodeFPS);
		double dblFPS = pIFloatFPS->GetValue();
		itoa(int(dblFPS), str, 10);
		Str = gcnew System::String(str);
		this->userControlOfCameraSetting_001->label_FPSValue->Text = Str;
	}
	else
	{
		this->userControlOfCameraSetting_001->label_ExposureValue->Text = "NULL";
		this->userControlOfCameraSetting_001->label_GainValue->Text = "NULL";
		this->userControlOfCameraSetting_001->label_FPSValue->Text = "NULL";
	}
}

System::Void UI::MyForm::pictureBox_ShowImage_Click(System::Object^  sender, System::EventArgs^  e)
{
	if (OpenCVObject.PresentMat.empty() == 0)
	{
		System::String^ Str;
		char firststr[20] = "./Appendix/Image/";
		char secondstr[20];
		char thirdstr[20] = ".bmp";

		SpecialNumber++;

		itoa(SpecialNumber, secondstr, 10);
		strcat(firststr, secondstr);
		strcat(firststr, thirdstr);

		imwrite(firststr, OpenCVObject.PresentMat);
	}
}

System::Void UI::MyForm::numericUpDown_Threshold_ValueChanged(System::Object^  sender, System::EventArgs^  e)
{
	if (OpenCVObject.PresentMat.empty() == 0)
	{
		OpenCVObject.ThresholdValue = Convert::ToInt32(this->numericUpDown_Threshold->Value);

		OpenCVObject.TemporaryMat.release();
		OpenCVObject.TemporaryMat = OpenCVObject.PresentMat.clone();
		cvtColor(OpenCVObject.TemporaryMat, OpenCVObject.TemporaryMat, CV_BGR2GRAY);
		threshold(OpenCVObject.TemporaryMat, OpenCVObject.TemporaryMat, OpenCVObject.ThresholdValue, 255, THRESH_BINARY);
		cvtColor(OpenCVObject.TemporaryMat, OpenCVObject.TemporaryMat, CV_GRAY2BGR);

		delete ShowCameraImage;
		ShowCameraImage = gcnew System::Drawing::Bitmap(OpenCVObject.TemporaryMat.cols, OpenCVObject.TemporaryMat.rows, OpenCVObject.TemporaryMat.step, System::Drawing::Imaging::PixelFormat::Format24bppRgb, System::IntPtr(OpenCVObject.TemporaryMat.data));
		this->pictureBox_ShowImage->Image = ShowCameraImage;
	}
	else
	{
		MessageBox::Show("Error Occur!");
	}
}

System::Void UI::MyForm::numericUpDown_DP_ValueChanged(System::Object^  sender, System::EventArgs^  e)
{
	OpenCVObject.HoughCircleDP = Convert::ToInt32(this->numericUpDown_DP->Value); 
}

System::Void UI::MyForm::numericUpDown_Parameter002_ValueChanged(System::Object^  sender, System::EventArgs^  e)
{
	OpenCVObject.HoughCircleParameter002 = Convert::ToInt32(this->numericUpDown_Parameter002->Value); 
}

System::Void UI::MyForm::numericUpDown_Parameter003_ValueChanged(System::Object^  sender, System::EventArgs^  e)
{
	OpenCVObject.HoughCircleParameter003 = Convert::ToInt32(this->numericUpDown_Parameter003->Value); 
}

System::Void UI::MyForm::numericUpDown_MinRadius_ValueChanged(System::Object^  sender, System::EventArgs^  e)
{
	OpenCVObject.HoughCircleMinRadius = Convert::ToInt32(this->numericUpDown_MinRadius->Value);
}

System::Void UI::MyForm::numericUpDown_MaxRadius_ValueChanged(System::Object^  sender, System::EventArgs^  e)
{
	OpenCVObject.HoughCircleMaxRadius = Convert::ToInt32(this->numericUpDown_MaxRadius->Value);
}
#pragma endregion

#pragma region Camera
void UI::ClassSTCCamera::Initial()
{

}

void UI::ClassSTCCamera::Grab()
{
	Mat inputMat;
	Mat displayMat;
	Mat *pMat;

	// Start the image acquisition of the host side.
	pIStDataStreamList.StartAcquisition(1);//取像無限幀數:GENTL_INFINITE

	// Start the image acquisition of the camera side.
	pIStDeviceList.AcquisitionStart();

	try
	{
		// A while loop for acquiring data and checking status. 
		// Here we use DataStream list function to check if any cameras in the list is on grabbing.
		while (pIStDataStreamList.IsGrabbingAny())
		{
			if (ImageNumber == 10000)
				ImageNumber = 0;
			else
				ImageNumber++;

			// Retrieve data buffer pointer of image data from any camera with a timeout of 10000ms.
			CIStStreamBufferPtr pIStStreamBuffer(pIStDataStreamList.RetrieveBuffer(10000));

			if (pIStStreamBuffer->GetIStStreamBufferInfo()->IsImagePresent())
			{
				IStImage *pIStImage = pIStStreamBuffer->GetIStImage();

				// IStImage轉成Mat
				// Check the pixelfomat of the input image.
				const StApi::EStPixelFormatNamingConvention_t ePFNC = pIStImage->GetImagePixelFormat();
				StApi::IStPixelFormatInfo * const pIStPixelFormatInfo = StApi::GetIStPixelFormatInfo(ePFNC);
				if (pIStPixelFormatInfo->IsMono() || pIStPixelFormatInfo->IsBayer())
				{
					// Check the size of the image.
					const size_t nImageWidth = pIStImage->GetImageWidth();
					const size_t nImageHeight = pIStImage->GetImageHeight();
					int nInputType = CV_8UC1;
					if (8 < pIStPixelFormatInfo->GetEachComponentTotalBitCount())
					{
						nInputType = CV_16UC1;
					}

					// Create a OpenCV buffer for the input image.
					if ((inputMat.cols != nImageWidth) || (inputMat.rows != nImageHeight) || (inputMat.type() != nInputType))
					{
						inputMat.create(nImageHeight, nImageWidth, nInputType);
					}

					// Copy the input image data to the buffer for OpenCV.
					const size_t dwBufferSize = inputMat.rows * inputMat.cols * inputMat.elemSize() * inputMat.channels();
					memcpy(inputMat.ptr(0), pIStImage->GetImageBuffer(), dwBufferSize);

					// Convert the pixelformat if needed.
					pMat = &inputMat;
					if (pIStPixelFormatInfo->IsBayer())
					{
						int nConvert = 0;
						switch (pIStPixelFormatInfo->GetPixelColorFilter())
						{
						case(StPixelColorFilter_BayerRG): nConvert = CV_BayerRG2RGB;	break;
						case(StPixelColorFilter_BayerGR): nConvert = CV_BayerGR2RGB;	break;
						case(StPixelColorFilter_BayerGB): nConvert = CV_BayerGB2RGB;	break;
						case(StPixelColorFilter_BayerBG): nConvert = CV_BayerBG2RGB;	break;
						}
						if (nConvert != 0)
						{
							cv::cvtColor(inputMat, displayMat, nConvert);
							pMat = &displayMat;
						}
					}
				}

				if (pMat->channels() == 1)
					cv::cvtColor(*pMat, *pMat, CV_GRAY2BGR);

				imshow("GrabImage", *pMat);
			}
			else
			{

			}
		}
	}
	catch (System::Exception ^ e)
	{

	}

	// Stop the image acquisition of the camera side.
	pIStDeviceList.AcquisitionStop();

	// Stop the image acquisition of the host side.
	pIStDataStreamList.StopAcquisition();
}

void STCCameraInitial()
{
	for (;;)
	{
		IStDeviceReleasable *pIStDeviceReleasable = NULL;

		try
		{
			pIStDeviceReleasable = pIStSystem->CreateFirstIStDevice();
		}
		catch (...)
		{
			if (pIStDeviceList.GetSize() == 0)
			{
				STCCameraErrorFlag = true;
				break;
			}
			else
			{
				break;
			}
		}

		pIStDeviceList.Register(pIStDeviceReleasable);
		pIStDataStreamList.Register(pIStDeviceReleasable->CreateIStDataStream(0));
	}

	if (STCCameraErrorFlag)
		std::cout << "攝影機初始化失敗" << endl;
	else
		std::cout << "攝影機數量 : " << pIStDeviceList.GetSize() << endl;
}
#pragma endregion