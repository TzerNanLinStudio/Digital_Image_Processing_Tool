#pragma once

#include <iostream>
#include <sstream>
#include <cstdint>
#include <opencv2/opencv.hpp>
#include <opencv2/imgproc/imgproc.hpp>

using namespace cv;

class ImageCore {
public:
    ImageCore();
    
    void SetOriginalImage(std::string path);

    void ShowImage(std::string input);

    System::Drawing::Bitmap^ GetImage(std::string input);

    void Recover();

    void Grayscale();

    void Binarize(std::string option, int value);

    void Dilate(int value);

    void Erode(int value);

    void MedianFiltering(int value);

    void Denoise(int value_01, int value_02, int value_03);

private:
    Mat originalImage;
    Mat presentImage;
    Mat temporaryImage;
};
