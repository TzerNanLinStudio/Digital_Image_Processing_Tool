#include "Data.h"

ImageCore::ImageCore() {

}

void ImageCore::SetOriginalImage(std::string path) {
	originalImage = imread(path, CV_LOAD_IMAGE_UNCHANGED);
	Recover();
}

void ImageCore::ShowImage(std::string input) {
	Mat image;

	if (input == "original")
		image = originalImage;
	else if (input == "present")
		image = presentImage;
	else if (input == "temporary")
		image = temporaryImage;
	else
		throw std::runtime_error("Unexpected Input.");

	cv::namedWindow("Image Display", cv::WINDOW_AUTOSIZE);
	cv::imshow("Image Display", image);
	cv::waitKey(0); 
}

System::Drawing::Bitmap^ ImageCore::GetImage(std::string input) {
	if (input == "original") {
		if (originalImage.channels() == 1)
			cvtColor(originalImage, originalImage, CV_GRAY2BGR);
		return gcnew System::Drawing::Bitmap(originalImage.cols, originalImage.rows, originalImage.step, System::Drawing::Imaging::PixelFormat::Format24bppRgb, System::IntPtr(originalImage.data));
	}
	else if (input == "present") {
		if (presentImage.channels() == 1)
			cvtColor(presentImage, presentImage, CV_GRAY2BGR);
		return gcnew System::Drawing::Bitmap(presentImage.cols, presentImage.rows, presentImage.step, System::Drawing::Imaging::PixelFormat::Format24bppRgb, System::IntPtr(presentImage.data));
	}
	else if (input == "temporary") {
		if (temporaryImage.channels() == 1)
			cvtColor(temporaryImage, temporaryImage, CV_GRAY2BGR);
		return gcnew System::Drawing::Bitmap(temporaryImage.cols, temporaryImage.rows, temporaryImage.step, System::Drawing::Imaging::PixelFormat::Format24bppRgb, System::IntPtr(temporaryImage.data));
	}
	else
		throw std::runtime_error("Unexpected Input.");
}

void ImageCore::Recover() {
	presentImage = originalImage.clone();
	temporaryImage.release();
}

void ImageCore::Grayscale() {
	cvtColor(presentImage, presentImage, CV_BGR2GRAY);
}

void ImageCore::Binarize(std::string option, int value) {
	if (option == "test") {
		temporaryImage = presentImage.clone();
		if (temporaryImage.channels() == 3)
			cvtColor(temporaryImage, temporaryImage, CV_BGR2GRAY);
		threshold(temporaryImage, temporaryImage, value, 255, THRESH_BINARY);
	}
	else if (option == "apply") {
		if (presentImage.channels() == 3)
			cvtColor(presentImage, presentImage, CV_BGR2GRAY);
		threshold(presentImage, presentImage, value, 255, THRESH_BINARY);
	}
	else
		throw std::runtime_error("Unexpected Option.");
}

void ImageCore::Dilate(int value) {
	dilate(presentImage, presentImage, value);
}

void ImageCore::Erode(int value) {
	erode(presentImage, presentImage, value);
}

void ImageCore::MedianFiltering(int value) {
	cv::medianBlur(presentImage, presentImage, value);
}

void ImageCore::Denoise(int value_01, int value_02, int value_03) {
	cv::fastNlMeansDenoisingColored(presentImage, presentImage, value_01, value_02, value_03);
}