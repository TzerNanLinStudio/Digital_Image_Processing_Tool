#pragma once
#include <windows.h>
#include <opencv2/opencv.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <cstdio>
#include <cmath>
#include <iostream>
#include <string> 
#include <conio.h>
#include <stdio.h>
#include <math.h> 
#include <time.h>
#include <direct.h>
#include <msclr/marshal_cppstd.h> 
#include <msclr/marshal.h>
#include <StApi_TL.h>
#include <StApi_GUI.h>
#include "LoginForm.h"

#define Mode_Of_Debug 0
#define Mode_Of_Solution 0;

#pragma comment (lib, "User32.lib")

namespace UI
{
	using namespace System;
	using namespace System::ComponentModel;
	using namespace System::Collections;
	using namespace System::Windows::Forms;
	using namespace System::Data;
	using namespace System::Drawing;
	using namespace System::Drawing::Imaging;
	using namespace System::Threading;
	using namespace System::Threading::Tasks;
	using namespace System::Runtime::InteropServices;
	using namespace std;
	using namespace cv;
	using namespace StApi;
	using namespace GenApi; 
	using namespace CameraSetting;

	[System::Runtime::InteropServices::ComVisible(false)]

	/// <summary>
	/// MyForm 的摘要
	/// </summary>
	public ref class MyForm : public System::Windows::Forms::Form
	{
	public:
		bool VideoRunFlag = false;
		bool DeveloperModeFlag = false;

	public:
		System::Drawing::Bitmap ^ ShowCameraImage = nullptr;
		System::Drawing::Bitmap ^ OriginImage = nullptr;

	public:
		MyForm(void)
		{
			InitializeComponent();
			//
			//TODO:  在此加入建構函式程式碼
			//
		}

	protected:
		/// <summary>
		/// 清除任何使用中的資源。
		/// </summary>
		~MyForm()
		{
			if (components)
			{
				delete components;
			}
		}
	private: System::Windows::Forms::TabControl^ tabControl_001;
	protected:


	private: System::Windows::Forms::TabPage^  tabPage1;
	private: System::Windows::Forms::TabPage^  tabPage2;
	private: CameraSetting::UserControlOfCameraSetting^  userControlOfCameraSetting_001;
	private: System::Windows::Forms::Button^  button_Origin;
	private: System::Windows::Forms::GroupBox^  groupBox_ImageBuffer002;
	private: System::Windows::Forms::Button^  button_ImageBuffer002_Register;
	private: System::Windows::Forms::Button^  button_ImageBuffer002_Show;
	private: System::Windows::Forms::NumericUpDown^  numericUpDown_Parameter003;
	private: System::Windows::Forms::Label^  label_005;
	private: System::Windows::Forms::GroupBox^  groupBox_ImageBuffer001;
	private: System::Windows::Forms::Button^  button_ImageBuffer001_Register;
	private: System::Windows::Forms::Button^  button_ImageBuffer001_Show;
	private: System::Windows::Forms::TabControl^  tabControl_ImformationLog;
	private: System::Windows::Forms::TabPage^  tabPage_GeneralLog;
	private: System::Windows::Forms::RichTextBox^  richTextBox_GeneralLog;
	private: System::Windows::Forms::TabPage^  tabPage_WarmingLog;
	private: System::Windows::Forms::RichTextBox^  richTextBox_WarmingLog;
	private: System::Windows::Forms::TabPage^  tabPage_ErrorLog;
	private: System::Windows::Forms::RichTextBox^  richTextBox_ErrorLog;
	private: System::Windows::Forms::MenuStrip^  menuStrip_001;
	private: System::Windows::Forms::ToolStripMenuItem^  ToolStripMenuItem_Mode;
	private: System::Windows::Forms::ToolStripMenuItem^  ToolStripMenuItem_UserMode;
	private: System::Windows::Forms::ToolStripMenuItem^  ToolStripMenuItem_DeveloperMode;
	private: System::Windows::Forms::Button^  button_001;
	private: System::Windows::Forms::Button^  button_002;
	private: System::Windows::Forms::Button^  button_Save;
	private: System::Windows::Forms::Button^  button_Open;
	private: System::Windows::Forms::OpenFileDialog^  openFileDialog_001;
	private: System::Windows::Forms::Label^  label_002;
	private: System::Windows::Forms::GroupBox^  groupBox_HoughCircle;
	private: System::Windows::Forms::NumericUpDown^  numericUpDown_MaxRadius;
	private: System::Windows::Forms::NumericUpDown^  numericUpDown_MinRadius;
	private: System::Windows::Forms::NumericUpDown^  numericUpDown_Parameter002;
	private: System::Windows::Forms::NumericUpDown^  numericUpDown_Parameter001;
	private: System::Windows::Forms::Label^  label_007;
	private: System::Windows::Forms::Label^  label_006;
	private: System::Windows::Forms::Label^  label_004;
	private: System::Windows::Forms::Label^  label_003;
	private: System::Windows::Forms::Button^  button_ApplyHoughCircle;
	private: System::Windows::Forms::Timer^  timer_UpdateUI;
	private: System::Windows::Forms::Button^  button_GrabStartAndStop;
	private: System::Windows::Forms::PictureBox^  pictureBox_ShowImage;
	private: System::Windows::Forms::Label^  label_001;
	private: System::ComponentModel::IContainer^  components;
	private: System::Windows::Forms::Button^  button_Gray;
	private: System::Windows::Forms::NumericUpDown^  numericUpDown_Threshold;
	private: System::Windows::Forms::GroupBox^  groupBox_Binarization;
	private: System::Windows::Forms::Button^  button_ApplyThreshold;

	private:
		/// <summary>
		/// 設計工具所需的變數。
		/// </summary>

#pragma region Windows Form Designer generated code
		/// <summary>
		/// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
		/// 這個方法的內容。
		/// </summary>
		void InitializeComponent(void)
		{
			this->components = (gcnew System::ComponentModel::Container());
			System::ComponentModel::ComponentResourceManager^ resources = (gcnew System::ComponentModel::ComponentResourceManager(MyForm::typeid));
			this->button_GrabStartAndStop = (gcnew System::Windows::Forms::Button());
			this->pictureBox_ShowImage = (gcnew System::Windows::Forms::PictureBox());
			this->label_001 = (gcnew System::Windows::Forms::Label());
			this->button_Gray = (gcnew System::Windows::Forms::Button());
			this->numericUpDown_Threshold = (gcnew System::Windows::Forms::NumericUpDown());
			this->groupBox_Binarization = (gcnew System::Windows::Forms::GroupBox());
			this->label_002 = (gcnew System::Windows::Forms::Label());
			this->button_ApplyThreshold = (gcnew System::Windows::Forms::Button());
			this->timer_UpdateUI = (gcnew System::Windows::Forms::Timer(this->components));
			this->menuStrip_001 = (gcnew System::Windows::Forms::MenuStrip());
			this->ToolStripMenuItem_Mode = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->ToolStripMenuItem_UserMode = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->ToolStripMenuItem_DeveloperMode = (gcnew System::Windows::Forms::ToolStripMenuItem());
			this->button_001 = (gcnew System::Windows::Forms::Button());
			this->button_002 = (gcnew System::Windows::Forms::Button());
			this->button_Save = (gcnew System::Windows::Forms::Button());
			this->button_Open = (gcnew System::Windows::Forms::Button());
			this->openFileDialog_001 = (gcnew System::Windows::Forms::OpenFileDialog());
			this->groupBox_HoughCircle = (gcnew System::Windows::Forms::GroupBox());
			this->numericUpDown_Parameter003 = (gcnew System::Windows::Forms::NumericUpDown());
			this->label_005 = (gcnew System::Windows::Forms::Label());
			this->button_ApplyHoughCircle = (gcnew System::Windows::Forms::Button());
			this->numericUpDown_MaxRadius = (gcnew System::Windows::Forms::NumericUpDown());
			this->numericUpDown_MinRadius = (gcnew System::Windows::Forms::NumericUpDown());
			this->numericUpDown_Parameter002 = (gcnew System::Windows::Forms::NumericUpDown());
			this->numericUpDown_Parameter001 = (gcnew System::Windows::Forms::NumericUpDown());
			this->label_007 = (gcnew System::Windows::Forms::Label());
			this->label_006 = (gcnew System::Windows::Forms::Label());
			this->label_004 = (gcnew System::Windows::Forms::Label());
			this->label_003 = (gcnew System::Windows::Forms::Label());
			this->groupBox_ImageBuffer001 = (gcnew System::Windows::Forms::GroupBox());
			this->button_ImageBuffer001_Register = (gcnew System::Windows::Forms::Button());
			this->button_ImageBuffer001_Show = (gcnew System::Windows::Forms::Button());
			this->tabControl_ImformationLog = (gcnew System::Windows::Forms::TabControl());
			this->tabPage_GeneralLog = (gcnew System::Windows::Forms::TabPage());
			this->richTextBox_GeneralLog = (gcnew System::Windows::Forms::RichTextBox());
			this->tabPage_WarmingLog = (gcnew System::Windows::Forms::TabPage());
			this->richTextBox_WarmingLog = (gcnew System::Windows::Forms::RichTextBox());
			this->tabPage_ErrorLog = (gcnew System::Windows::Forms::TabPage());
			this->richTextBox_ErrorLog = (gcnew System::Windows::Forms::RichTextBox());
			this->button_Origin = (gcnew System::Windows::Forms::Button());
			this->groupBox_ImageBuffer002 = (gcnew System::Windows::Forms::GroupBox());
			this->button_ImageBuffer002_Register = (gcnew System::Windows::Forms::Button());
			this->button_ImageBuffer002_Show = (gcnew System::Windows::Forms::Button());
			this->tabControl_001 = (gcnew System::Windows::Forms::TabControl());
			this->tabPage1 = (gcnew System::Windows::Forms::TabPage());
			this->tabPage2 = (gcnew System::Windows::Forms::TabPage());
			this->userControlOfCameraSetting_001 = (gcnew CameraSetting::UserControlOfCameraSetting());
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^>(this->pictureBox_ShowImage))->BeginInit();
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^>(this->numericUpDown_Threshold))->BeginInit();
			this->groupBox_Binarization->SuspendLayout();
			this->menuStrip_001->SuspendLayout();
			this->groupBox_HoughCircle->SuspendLayout();
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^>(this->numericUpDown_Parameter003))->BeginInit();
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^>(this->numericUpDown_MaxRadius))->BeginInit();
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^>(this->numericUpDown_MinRadius))->BeginInit();
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^>(this->numericUpDown_Parameter002))->BeginInit();
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^>(this->numericUpDown_Parameter001))->BeginInit();
			this->groupBox_ImageBuffer001->SuspendLayout();
			this->tabControl_ImformationLog->SuspendLayout();
			this->tabPage_GeneralLog->SuspendLayout();
			this->tabPage_WarmingLog->SuspendLayout();
			this->tabPage_ErrorLog->SuspendLayout();
			this->groupBox_ImageBuffer002->SuspendLayout();
			this->tabControl_001->SuspendLayout();
			this->SuspendLayout();
			// 
			// button_GrabStartAndStop
			// 
			this->button_GrabStartAndStop->Font = (gcnew System::Drawing::Font(L"微軟正黑體", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->button_GrabStartAndStop->Location = System::Drawing::Point(1228, 329);
			this->button_GrabStartAndStop->Name = L"button_GrabStartAndStop";
			this->button_GrabStartAndStop->Size = System::Drawing::Size(150, 50);
			this->button_GrabStartAndStop->TabIndex = 0;
			this->button_GrabStartAndStop->Text = L"Start";
			this->button_GrabStartAndStop->UseVisualStyleBackColor = true;
			this->button_GrabStartAndStop->Click += gcnew System::EventHandler(this, &MyForm::button_GrabStartAndStop_Click);
			// 
			// pictureBox_ShowImage
			// 
			this->pictureBox_ShowImage->BackColor = System::Drawing::Color::Black;
			this->pictureBox_ShowImage->Location = System::Drawing::Point(10, 46);
			this->pictureBox_ShowImage->Name = L"pictureBox_ShowImage";
			this->pictureBox_ShowImage->Size = System::Drawing::Size(1200, 800);
			this->pictureBox_ShowImage->SizeMode = System::Windows::Forms::PictureBoxSizeMode::Zoom;
			this->pictureBox_ShowImage->TabIndex = 2;
			this->pictureBox_ShowImage->TabStop = false;
			this->pictureBox_ShowImage->Click += gcnew System::EventHandler(this, &MyForm::pictureBox_ShowImage_Click);
			// 
			// label_001
			// 
			this->label_001->AutoSize = true;
			this->label_001->Font = (gcnew System::Drawing::Font(L"微軟正黑體", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->label_001->Location = System::Drawing::Point(970, 849);
			this->label_001->Name = L"label_001";
			this->label_001->Size = System::Drawing::Size(197, 25);
			this->label_001->TabIndex = 3;
			this->label_001->Text = L"Receiced Frame = 0";
			// 
			// button_Gray
			// 
			this->button_Gray->Font = (gcnew System::Drawing::Font(L"微軟正黑體", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->button_Gray->Location = System::Drawing::Point(1384, 270);
			this->button_Gray->Name = L"button_Gray";
			this->button_Gray->Size = System::Drawing::Size(150, 50);
			this->button_Gray->TabIndex = 4;
			this->button_Gray->Text = L"Gray";
			this->button_Gray->UseVisualStyleBackColor = true;
			this->button_Gray->Click += gcnew System::EventHandler(this, &MyForm::button_Gray_Click);
			// 
			// numericUpDown_Threshold
			// 
			this->numericUpDown_Threshold->Location = System::Drawing::Point(165, 26);
			this->numericUpDown_Threshold->Maximum = System::Decimal(gcnew cli::array< System::Int32 >(4) { 255, 0, 0, 0 });
			this->numericUpDown_Threshold->Name = L"numericUpDown_Threshold";
			this->numericUpDown_Threshold->Size = System::Drawing::Size(150, 34);
			this->numericUpDown_Threshold->TabIndex = 5;
			this->numericUpDown_Threshold->ValueChanged += gcnew System::EventHandler(this, &MyForm::numericUpDown_Threshold_ValueChanged);
			// 
			// groupBox_Binarization
			// 
			this->groupBox_Binarization->Controls->Add(this->label_002);
			this->groupBox_Binarization->Controls->Add(this->button_ApplyThreshold);
			this->groupBox_Binarization->Controls->Add(this->numericUpDown_Threshold);
			this->groupBox_Binarization->Font = (gcnew System::Drawing::Font(L"微軟正黑體", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->groupBox_Binarization->Location = System::Drawing::Point(1228, 381);
			this->groupBox_Binarization->Name = L"groupBox_Binarization";
			this->groupBox_Binarization->Size = System::Drawing::Size(330, 130);
			this->groupBox_Binarization->TabIndex = 6;
			this->groupBox_Binarization->TabStop = false;
			this->groupBox_Binarization->Text = L"Binarization";
			// 
			// label_002
			// 
			this->label_002->AutoSize = true;
			this->label_002->Font = (gcnew System::Drawing::Font(L"微軟正黑體", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->label_002->Location = System::Drawing::Point(6, 35);
			this->label_002->Name = L"label_002";
			this->label_002->Size = System::Drawing::Size(140, 25);
			this->label_002->TabIndex = 14;
			this->label_002->Text = L"Threshold      :";
			// 
			// button_ApplyThreshold
			// 
			this->button_ApplyThreshold->Location = System::Drawing::Point(165, 70);
			this->button_ApplyThreshold->Name = L"button_ApplyThreshold";
			this->button_ApplyThreshold->Size = System::Drawing::Size(150, 50);
			this->button_ApplyThreshold->TabIndex = 7;
			this->button_ApplyThreshold->Text = L"Apply";
			this->button_ApplyThreshold->UseVisualStyleBackColor = true;
			this->button_ApplyThreshold->Click += gcnew System::EventHandler(this, &MyForm::button_ApplyThreshold_Click);
			// 
			// timer_UpdateUI
			// 
			this->timer_UpdateUI->Enabled = true;
			this->timer_UpdateUI->Interval = 1000;
			this->timer_UpdateUI->Tick += gcnew System::EventHandler(this, &MyForm::timer_UpdateUI_Tick);
			// 
			// menuStrip_001
			// 
			this->menuStrip_001->Font = (gcnew System::Drawing::Font(L"微軟正黑體", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->menuStrip_001->Items->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(1) { this->ToolStripMenuItem_Mode });
			this->menuStrip_001->Location = System::Drawing::Point(0, 0);
			this->menuStrip_001->Name = L"menuStrip_001";
			this->menuStrip_001->Size = System::Drawing::Size(1904, 33);
			this->menuStrip_001->TabIndex = 8;
			this->menuStrip_001->Text = L"menuStrip1";
			// 
			// ToolStripMenuItem_Mode
			// 
			this->ToolStripMenuItem_Mode->DropDownItems->AddRange(gcnew cli::array< System::Windows::Forms::ToolStripItem^  >(2) {
				this->ToolStripMenuItem_UserMode,
					this->ToolStripMenuItem_DeveloperMode
			});
			this->ToolStripMenuItem_Mode->Name = L"ToolStripMenuItem_Mode";
			this->ToolStripMenuItem_Mode->Size = System::Drawing::Size(80, 29);
			this->ToolStripMenuItem_Mode->Text = L"Mode";
			// 
			// ToolStripMenuItem_UserMode
			// 
			this->ToolStripMenuItem_UserMode->Name = L"ToolStripMenuItem_UserMode";
			this->ToolStripMenuItem_UserMode->Size = System::Drawing::Size(180, 30);
			this->ToolStripMenuItem_UserMode->Text = L"User";
			this->ToolStripMenuItem_UserMode->Click += gcnew System::EventHandler(this, &MyForm::ToolStripMenuItem_UserMode_Click);
			// 
			// ToolStripMenuItem_DeveloperMode
			// 
			this->ToolStripMenuItem_DeveloperMode->Name = L"ToolStripMenuItem_DeveloperMode";
			this->ToolStripMenuItem_DeveloperMode->Size = System::Drawing::Size(180, 30);
			this->ToolStripMenuItem_DeveloperMode->Text = L"Developer";
			this->ToolStripMenuItem_DeveloperMode->Click += gcnew System::EventHandler(this, &MyForm::ToolStripMenuItem_DeveloperMode_Click);
			// 
			// button_001
			// 
			this->button_001->Font = (gcnew System::Drawing::Font(L"微軟正黑體", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->button_001->Location = System::Drawing::Point(1540, 214);
			this->button_001->Name = L"button_001";
			this->button_001->Size = System::Drawing::Size(150, 50);
			this->button_001->TabIndex = 9;
			this->button_001->Text = L"Test(1)";
			this->button_001->UseVisualStyleBackColor = true;
			this->button_001->Visible = false;
			this->button_001->Click += gcnew System::EventHandler(this, &MyForm::button_001_Click);
			// 
			// button_002
			// 
			this->button_002->Font = (gcnew System::Drawing::Font(L"微軟正黑體", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->button_002->Location = System::Drawing::Point(1540, 270);
			this->button_002->Name = L"button_002";
			this->button_002->Size = System::Drawing::Size(150, 50);
			this->button_002->TabIndex = 10;
			this->button_002->Text = L"Test(2)";
			this->button_002->UseVisualStyleBackColor = true;
			this->button_002->Visible = false;
			this->button_002->Click += gcnew System::EventHandler(this, &MyForm::button_002_Click);
			// 
			// button_Save
			// 
			this->button_Save->Font = (gcnew System::Drawing::Font(L"微軟正黑體", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->button_Save->Location = System::Drawing::Point(1384, 214);
			this->button_Save->Name = L"button_Save";
			this->button_Save->Size = System::Drawing::Size(150, 50);
			this->button_Save->TabIndex = 11;
			this->button_Save->Text = L"Save";
			this->button_Save->UseVisualStyleBackColor = true;
			this->button_Save->Click += gcnew System::EventHandler(this, &MyForm::button_Save_Click);
			// 
			// button_Open
			// 
			this->button_Open->Font = (gcnew System::Drawing::Font(L"微軟正黑體", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->button_Open->Location = System::Drawing::Point(1228, 214);
			this->button_Open->Name = L"button_Open";
			this->button_Open->Size = System::Drawing::Size(150, 50);
			this->button_Open->TabIndex = 12;
			this->button_Open->Text = L"Open";
			this->button_Open->UseVisualStyleBackColor = true;
			this->button_Open->Click += gcnew System::EventHandler(this, &MyForm::button_Open_Click);
			// 
			// openFileDialog_001
			// 
			this->openFileDialog_001->FileName = L"openFileDialog1";
			// 
			// groupBox_HoughCircle
			// 
			this->groupBox_HoughCircle->Controls->Add(this->numericUpDown_Parameter003);
			this->groupBox_HoughCircle->Controls->Add(this->label_005);
			this->groupBox_HoughCircle->Controls->Add(this->button_ApplyHoughCircle);
			this->groupBox_HoughCircle->Controls->Add(this->numericUpDown_MaxRadius);
			this->groupBox_HoughCircle->Controls->Add(this->numericUpDown_MinRadius);
			this->groupBox_HoughCircle->Controls->Add(this->numericUpDown_Parameter002);
			this->groupBox_HoughCircle->Controls->Add(this->numericUpDown_Parameter001);
			this->groupBox_HoughCircle->Controls->Add(this->label_007);
			this->groupBox_HoughCircle->Controls->Add(this->label_006);
			this->groupBox_HoughCircle->Controls->Add(this->label_004);
			this->groupBox_HoughCircle->Controls->Add(this->label_003);
			this->groupBox_HoughCircle->Font = (gcnew System::Drawing::Font(L"微軟正黑體", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->groupBox_HoughCircle->Location = System::Drawing::Point(1228, 517);
			this->groupBox_HoughCircle->Name = L"groupBox_HoughCircle";
			this->groupBox_HoughCircle->Size = System::Drawing::Size(330, 310);
			this->groupBox_HoughCircle->TabIndex = 13;
			this->groupBox_HoughCircle->TabStop = false;
			this->groupBox_HoughCircle->Text = L"Hough Circle";
			// 
			// numericUpDown_Parameter003
			// 
			this->numericUpDown_Parameter003->Location = System::Drawing::Point(165, 119);
			this->numericUpDown_Parameter003->Maximum = System::Decimal(gcnew cli::array< System::Int32 >(4) { 10000, 0, 0, 0 });
			this->numericUpDown_Parameter003->Name = L"numericUpDown_Parameter003";
			this->numericUpDown_Parameter003->Size = System::Drawing::Size(150, 34);
			this->numericUpDown_Parameter003->TabIndex = 23;
			this->numericUpDown_Parameter003->Value = System::Decimal(gcnew cli::array< System::Int32 >(4) { 100, 0, 0, 0 });
			this->numericUpDown_Parameter003->ValueChanged += gcnew System::EventHandler(this, &MyForm::numericUpDown_Parameter003_ValueChanged);
			// 
			// label_005
			// 
			this->label_005->AutoSize = true;
			this->label_005->Font = (gcnew System::Drawing::Font(L"微軟正黑體", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->label_005->Location = System::Drawing::Point(6, 126);
			this->label_005->Name = L"label_005";
			this->label_005->Size = System::Drawing::Size(142, 25);
			this->label_005->TabIndex = 22;
			this->label_005->Text = L"Parameter(3) :";
			// 
			// button_ApplyHoughCircle
			// 
			this->button_ApplyHoughCircle->Location = System::Drawing::Point(165, 244);
			this->button_ApplyHoughCircle->Name = L"button_ApplyHoughCircle";
			this->button_ApplyHoughCircle->Size = System::Drawing::Size(150, 50);
			this->button_ApplyHoughCircle->TabIndex = 15;
			this->button_ApplyHoughCircle->Text = L"Apply";
			this->button_ApplyHoughCircle->UseVisualStyleBackColor = true;
			this->button_ApplyHoughCircle->Click += gcnew System::EventHandler(this, &MyForm::button_ApplyHoughCircle_Click);
			// 
			// numericUpDown_MaxRadius
			// 
			this->numericUpDown_MaxRadius->Location = System::Drawing::Point(165, 201);
			this->numericUpDown_MaxRadius->Maximum = System::Decimal(gcnew cli::array< System::Int32 >(4) { 10000, 0, 0, 0 });
			this->numericUpDown_MaxRadius->Name = L"numericUpDown_MaxRadius";
			this->numericUpDown_MaxRadius->Size = System::Drawing::Size(150, 34);
			this->numericUpDown_MaxRadius->TabIndex = 21;
			this->numericUpDown_MaxRadius->Value = System::Decimal(gcnew cli::array< System::Int32 >(4) { 100, 0, 0, 0 });
			this->numericUpDown_MaxRadius->ValueChanged += gcnew System::EventHandler(this, &MyForm::numericUpDown_MaxRadius_ValueChanged);
			// 
			// numericUpDown_MinRadius
			// 
			this->numericUpDown_MinRadius->Location = System::Drawing::Point(165, 160);
			this->numericUpDown_MinRadius->Maximum = System::Decimal(gcnew cli::array< System::Int32 >(4) { 10000, 0, 0, 0 });
			this->numericUpDown_MinRadius->Name = L"numericUpDown_MinRadius";
			this->numericUpDown_MinRadius->Size = System::Drawing::Size(150, 34);
			this->numericUpDown_MinRadius->TabIndex = 20;
			this->numericUpDown_MinRadius->Value = System::Decimal(gcnew cli::array< System::Int32 >(4) { 10, 0, 0, 0 });
			this->numericUpDown_MinRadius->ValueChanged += gcnew System::EventHandler(this, &MyForm::numericUpDown_MinRadius_ValueChanged);
			// 
			// numericUpDown_Parameter002
			// 
			this->numericUpDown_Parameter002->Location = System::Drawing::Point(165, 77);
			this->numericUpDown_Parameter002->Maximum = System::Decimal(gcnew cli::array< System::Int32 >(4) { 10000, 0, 0, 0 });
			this->numericUpDown_Parameter002->Name = L"numericUpDown_Parameter002";
			this->numericUpDown_Parameter002->Size = System::Drawing::Size(150, 34);
			this->numericUpDown_Parameter002->TabIndex = 19;
			this->numericUpDown_Parameter002->Value = System::Decimal(gcnew cli::array< System::Int32 >(4) { 100, 0, 0, 0 });
			this->numericUpDown_Parameter002->ValueChanged += gcnew System::EventHandler(this, &MyForm::numericUpDown_Parameter002_ValueChanged);
			// 
			// numericUpDown_Parameter001
			// 
			this->numericUpDown_Parameter001->Location = System::Drawing::Point(165, 33);
			this->numericUpDown_Parameter001->Maximum = System::Decimal(gcnew cli::array< System::Int32 >(4) { 10000, 0, 0, 0 });
			this->numericUpDown_Parameter001->Name = L"numericUpDown_Parameter001";
			this->numericUpDown_Parameter001->Size = System::Drawing::Size(150, 34);
			this->numericUpDown_Parameter001->TabIndex = 15;
			this->numericUpDown_Parameter001->Value = System::Decimal(gcnew cli::array< System::Int32 >(4) { 10, 0, 0, 0 });
			this->numericUpDown_Parameter001->ValueChanged += gcnew System::EventHandler(this, &MyForm::numericUpDown_Parameter001_ValueChanged);
			// 
			// label_007
			// 
			this->label_007->AutoSize = true;
			this->label_007->Font = (gcnew System::Drawing::Font(L"微軟正黑體", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->label_007->Location = System::Drawing::Point(6, 206);
			this->label_007->Name = L"label_007";
			this->label_007->Size = System::Drawing::Size(140, 25);
			this->label_007->TabIndex = 18;
			this->label_007->Text = L"MaxRadius    :";
			// 
			// label_006
			// 
			this->label_006->AutoSize = true;
			this->label_006->Font = (gcnew System::Drawing::Font(L"微軟正黑體", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->label_006->Location = System::Drawing::Point(6, 166);
			this->label_006->Name = L"label_006";
			this->label_006->Size = System::Drawing::Size(141, 25);
			this->label_006->TabIndex = 17;
			this->label_006->Text = L"MinRadius     :";
			// 
			// label_004
			// 
			this->label_004->AutoSize = true;
			this->label_004->Font = (gcnew System::Drawing::Font(L"微軟正黑體", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->label_004->Location = System::Drawing::Point(6, 84);
			this->label_004->Name = L"label_004";
			this->label_004->Size = System::Drawing::Size(142, 25);
			this->label_004->TabIndex = 16;
			this->label_004->Text = L"Parameter(2) :";
			// 
			// label_003
			// 
			this->label_003->AutoSize = true;
			this->label_003->Font = (gcnew System::Drawing::Font(L"微軟正黑體", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->label_003->Location = System::Drawing::Point(6, 39);
			this->label_003->Name = L"label_003";
			this->label_003->Size = System::Drawing::Size(142, 25);
			this->label_003->TabIndex = 15;
			this->label_003->Text = L"Parameter(1) :";
			// 
			// groupBox_ImageBuffer001
			// 
			this->groupBox_ImageBuffer001->Controls->Add(this->button_ImageBuffer001_Register);
			this->groupBox_ImageBuffer001->Controls->Add(this->button_ImageBuffer001_Show);
			this->groupBox_ImageBuffer001->Font = (gcnew System::Drawing::Font(L"微軟正黑體", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->groupBox_ImageBuffer001->Location = System::Drawing::Point(1228, 833);
			this->groupBox_ImageBuffer001->Name = L"groupBox_ImageBuffer001";
			this->groupBox_ImageBuffer001->Size = System::Drawing::Size(330, 100);
			this->groupBox_ImageBuffer001->TabIndex = 15;
			this->groupBox_ImageBuffer001->TabStop = false;
			this->groupBox_ImageBuffer001->Text = L"ImageBuffer(1)";
			// 
			// button_ImageBuffer001_Register
			// 
			this->button_ImageBuffer001_Register->Location = System::Drawing::Point(9, 33);
			this->button_ImageBuffer001_Register->Name = L"button_ImageBuffer001_Register";
			this->button_ImageBuffer001_Register->Size = System::Drawing::Size(150, 50);
			this->button_ImageBuffer001_Register->TabIndex = 8;
			this->button_ImageBuffer001_Register->Text = L"Register";
			this->button_ImageBuffer001_Register->UseVisualStyleBackColor = true;
			this->button_ImageBuffer001_Register->Click += gcnew System::EventHandler(this, &MyForm::button_ImageBuffer001_Register_Click);
			// 
			// button_ImageBuffer001_Show
			// 
			this->button_ImageBuffer001_Show->Location = System::Drawing::Point(165, 33);
			this->button_ImageBuffer001_Show->Name = L"button_ImageBuffer001_Show";
			this->button_ImageBuffer001_Show->Size = System::Drawing::Size(150, 50);
			this->button_ImageBuffer001_Show->TabIndex = 7;
			this->button_ImageBuffer001_Show->Text = L"Show";
			this->button_ImageBuffer001_Show->UseVisualStyleBackColor = true;
			this->button_ImageBuffer001_Show->Click += gcnew System::EventHandler(this, &MyForm::button_ImageBuffer001_Show_Click);
			// 
			// tabControl_ImformationLog
			// 
			this->tabControl_ImformationLog->Controls->Add(this->tabPage_GeneralLog);
			this->tabControl_ImformationLog->Controls->Add(this->tabPage_WarmingLog);
			this->tabControl_ImformationLog->Controls->Add(this->tabPage_ErrorLog);
			this->tabControl_ImformationLog->Font = (gcnew System::Drawing::Font(L"微軟正黑體", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->tabControl_ImformationLog->Location = System::Drawing::Point(10, 877);
			this->tabControl_ImformationLog->Name = L"tabControl_ImformationLog";
			this->tabControl_ImformationLog->SelectedIndex = 0;
			this->tabControl_ImformationLog->Size = System::Drawing::Size(1200, 152);
			this->tabControl_ImformationLog->TabIndex = 16;
			// 
			// tabPage_GeneralLog
			// 
			this->tabPage_GeneralLog->Controls->Add(this->richTextBox_GeneralLog);
			this->tabPage_GeneralLog->Location = System::Drawing::Point(4, 34);
			this->tabPage_GeneralLog->Name = L"tabPage_GeneralLog";
			this->tabPage_GeneralLog->Padding = System::Windows::Forms::Padding(3);
			this->tabPage_GeneralLog->Size = System::Drawing::Size(1192, 114);
			this->tabPage_GeneralLog->TabIndex = 0;
			this->tabPage_GeneralLog->Text = L"General";
			this->tabPage_GeneralLog->UseVisualStyleBackColor = true;
			// 
			// richTextBox_GeneralLog
			// 
			this->richTextBox_GeneralLog->Location = System::Drawing::Point(6, 6);
			this->richTextBox_GeneralLog->Name = L"richTextBox_GeneralLog";
			this->richTextBox_GeneralLog->Size = System::Drawing::Size(1180, 102);
			this->richTextBox_GeneralLog->TabIndex = 0;
			this->richTextBox_GeneralLog->Text = L"";
			// 
			// tabPage_WarmingLog
			// 
			this->tabPage_WarmingLog->Controls->Add(this->richTextBox_WarmingLog);
			this->tabPage_WarmingLog->Location = System::Drawing::Point(4, 34);
			this->tabPage_WarmingLog->Name = L"tabPage_WarmingLog";
			this->tabPage_WarmingLog->Padding = System::Windows::Forms::Padding(3);
			this->tabPage_WarmingLog->Size = System::Drawing::Size(1192, 114);
			this->tabPage_WarmingLog->TabIndex = 1;
			this->tabPage_WarmingLog->Text = L"Warming";
			this->tabPage_WarmingLog->UseVisualStyleBackColor = true;
			// 
			// richTextBox_WarmingLog
			// 
			this->richTextBox_WarmingLog->Location = System::Drawing::Point(6, 6);
			this->richTextBox_WarmingLog->Name = L"richTextBox_WarmingLog";
			this->richTextBox_WarmingLog->Size = System::Drawing::Size(1180, 102);
			this->richTextBox_WarmingLog->TabIndex = 17;
			this->richTextBox_WarmingLog->Text = L"";
			// 
			// tabPage_ErrorLog
			// 
			this->tabPage_ErrorLog->Controls->Add(this->richTextBox_ErrorLog);
			this->tabPage_ErrorLog->Location = System::Drawing::Point(4, 34);
			this->tabPage_ErrorLog->Name = L"tabPage_ErrorLog";
			this->tabPage_ErrorLog->Size = System::Drawing::Size(1192, 114);
			this->tabPage_ErrorLog->TabIndex = 2;
			this->tabPage_ErrorLog->Text = L"Error";
			this->tabPage_ErrorLog->UseVisualStyleBackColor = true;
			// 
			// richTextBox_ErrorLog
			// 
			this->richTextBox_ErrorLog->Location = System::Drawing::Point(9, 5);
			this->richTextBox_ErrorLog->Name = L"richTextBox_ErrorLog";
			this->richTextBox_ErrorLog->Size = System::Drawing::Size(1180, 102);
			this->richTextBox_ErrorLog->TabIndex = 17;
			this->richTextBox_ErrorLog->Text = L"";
			// 
			// button_Origin
			// 
			this->button_Origin->Font = (gcnew System::Drawing::Font(L"微軟正黑體", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->button_Origin->Location = System::Drawing::Point(1228, 270);
			this->button_Origin->Name = L"button_Origin";
			this->button_Origin->Size = System::Drawing::Size(150, 50);
			this->button_Origin->TabIndex = 17;
			this->button_Origin->Text = L"Origin";
			this->button_Origin->UseVisualStyleBackColor = true;
			this->button_Origin->Click += gcnew System::EventHandler(this, &MyForm::button_Origin_Click);
			// 
			// groupBox_ImageBuffer002
			// 
			this->groupBox_ImageBuffer002->Controls->Add(this->button_ImageBuffer002_Register);
			this->groupBox_ImageBuffer002->Controls->Add(this->button_ImageBuffer002_Show);
			this->groupBox_ImageBuffer002->Font = (gcnew System::Drawing::Font(L"微軟正黑體", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->groupBox_ImageBuffer002->Location = System::Drawing::Point(1228, 939);
			this->groupBox_ImageBuffer002->Name = L"groupBox_ImageBuffer002";
			this->groupBox_ImageBuffer002->Size = System::Drawing::Size(330, 100);
			this->groupBox_ImageBuffer002->TabIndex = 16;
			this->groupBox_ImageBuffer002->TabStop = false;
			this->groupBox_ImageBuffer002->Text = L"ImageBuffer(2)";
			// 
			// button_ImageBuffer002_Register
			// 
			this->button_ImageBuffer002_Register->Location = System::Drawing::Point(9, 33);
			this->button_ImageBuffer002_Register->Name = L"button_ImageBuffer002_Register";
			this->button_ImageBuffer002_Register->Size = System::Drawing::Size(150, 50);
			this->button_ImageBuffer002_Register->TabIndex = 8;
			this->button_ImageBuffer002_Register->Text = L"Register";
			this->button_ImageBuffer002_Register->UseVisualStyleBackColor = true;
			this->button_ImageBuffer002_Register->Click += gcnew System::EventHandler(this, &MyForm::button_ImageBuffer002_Register_Click);
			// 
			// button_ImageBuffer002_Show
			// 
			this->button_ImageBuffer002_Show->Location = System::Drawing::Point(165, 33);
			this->button_ImageBuffer002_Show->Name = L"button_ImageBuffer002_Show";
			this->button_ImageBuffer002_Show->Size = System::Drawing::Size(150, 50);
			this->button_ImageBuffer002_Show->TabIndex = 7;
			this->button_ImageBuffer002_Show->Text = L"Show";
			this->button_ImageBuffer002_Show->UseVisualStyleBackColor = true;
			this->button_ImageBuffer002_Show->Click += gcnew System::EventHandler(this, &MyForm::button_ImageBuffer002_Show_Click);
			// 
			// tabControl_001
			// 
			this->tabControl_001->Alignment = System::Windows::Forms::TabAlignment::Left;
			this->tabControl_001->Controls->Add(this->tabPage1);
			this->tabControl_001->Controls->Add(this->tabPage2);
			this->tabControl_001->Font = (gcnew System::Drawing::Font(L"微軟正黑體", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->tabControl_001->Location = System::Drawing::Point(1709, 47);
			this->tabControl_001->Multiline = true;
			this->tabControl_001->Name = L"tabControl_001";
			this->tabControl_001->SelectedIndex = 0;
			this->tabControl_001->Size = System::Drawing::Size(174, 273);
			this->tabControl_001->SizeMode = System::Windows::Forms::TabSizeMode::Fixed;
			this->tabControl_001->TabIndex = 19;
			this->tabControl_001->Visible = false;
			// 
			// tabPage1
			// 
			this->tabPage1->Location = System::Drawing::Point(37, 4);
			this->tabPage1->Name = L"tabPage1";
			this->tabPage1->Padding = System::Windows::Forms::Padding(3);
			this->tabPage1->Size = System::Drawing::Size(133, 265);
			this->tabPage1->TabIndex = 0;
			this->tabPage1->Text = L"Page 01";
			this->tabPage1->UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this->tabPage2->Location = System::Drawing::Point(37, 4);
			this->tabPage2->Name = L"tabPage2";
			this->tabPage2->Padding = System::Windows::Forms::Padding(3);
			this->tabPage2->Size = System::Drawing::Size(133, 265);
			this->tabPage2->TabIndex = 1;
			this->tabPage2->Text = L"Page 02";
			this->tabPage2->UseVisualStyleBackColor = true;
			// 
			// userControlOfCameraSetting_001
			// 
			this->userControlOfCameraSetting_001->BackColor = System::Drawing::Color::DeepSkyBlue;
			this->userControlOfCameraSetting_001->BackgroundImageLayout = System::Windows::Forms::ImageLayout::None;
			this->userControlOfCameraSetting_001->Location = System::Drawing::Point(1228, 47);
			this->userControlOfCameraSetting_001->Name = L"userControlOfCameraSetting_001";
			this->userControlOfCameraSetting_001->Size = System::Drawing::Size(450, 150);
			this->userControlOfCameraSetting_001->TabIndex = 18;
			// 
			// MyForm
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 12);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->BackColor = System::Drawing::Color::LightSkyBlue;
			this->ClientSize = System::Drawing::Size(1904, 1041);
			this->Controls->Add(this->tabControl_001);
			this->Controls->Add(this->userControlOfCameraSetting_001);
			this->Controls->Add(this->groupBox_ImageBuffer002);
			this->Controls->Add(this->button_Origin);
			this->Controls->Add(this->tabControl_ImformationLog);
			this->Controls->Add(this->groupBox_ImageBuffer001);
			this->Controls->Add(this->groupBox_HoughCircle);
			this->Controls->Add(this->button_Open);
			this->Controls->Add(this->button_Save);
			this->Controls->Add(this->button_002);
			this->Controls->Add(this->button_001);
			this->Controls->Add(this->groupBox_Binarization);
			this->Controls->Add(this->button_Gray);
			this->Controls->Add(this->label_001);
			this->Controls->Add(this->pictureBox_ShowImage);
			this->Controls->Add(this->button_GrabStartAndStop);
			this->Controls->Add(this->menuStrip_001);
			this->Font = (gcnew System::Drawing::Font(L"新細明體", 9, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->ForeColor = System::Drawing::SystemColors::ControlText;
			this->Icon = gcnew System::Drawing::Icon("Appendix\\Icon.ico"); //手寫加入
			this->MainMenuStrip = this->menuStrip_001;
			this->Name = L"MyForm";
			this->Text = L"Image Processing and Analysis";
			this->FormClosing += gcnew System::Windows::Forms::FormClosingEventHandler(this, &MyForm::MainForm_Closing);
			this->Load += gcnew System::EventHandler(this, &MyForm::MainForm_Load);
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^>(this->pictureBox_ShowImage))->EndInit();
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^>(this->numericUpDown_Threshold))->EndInit();
			this->groupBox_Binarization->ResumeLayout(false);
			this->groupBox_Binarization->PerformLayout();
			this->menuStrip_001->ResumeLayout(false);
			this->menuStrip_001->PerformLayout();
			this->groupBox_HoughCircle->ResumeLayout(false);
			this->groupBox_HoughCircle->PerformLayout();
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^>(this->numericUpDown_Parameter003))->EndInit();
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^>(this->numericUpDown_MaxRadius))->EndInit();
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^>(this->numericUpDown_MinRadius))->EndInit();
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^>(this->numericUpDown_Parameter002))->EndInit();
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^>(this->numericUpDown_Parameter001))->EndInit();
			this->groupBox_ImageBuffer001->ResumeLayout(false);
			this->tabControl_ImformationLog->ResumeLayout(false);
			this->tabPage_GeneralLog->ResumeLayout(false);
			this->tabPage_WarmingLog->ResumeLayout(false);
			this->tabPage_ErrorLog->ResumeLayout(false);
			this->groupBox_ImageBuffer002->ResumeLayout(false);
			this->tabControl_001->ResumeLayout(false);
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion

	private: System::Void MainForm_Load(System::Object^  sender, System::EventArgs^  e);
	private: System::Void MainForm_Closing(System::Object^  sender, System::Windows::Forms::FormClosingEventArgs ^ e);
	private: System::Void ToolStripMenuItem_UserMode_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void ToolStripMenuItem_DeveloperMode_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void button_Open_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void button_Save_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void button_GrabStartAndStop_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void button_Origin_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void button_Gray_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void button_ApplyThreshold_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void button_ApplyHoughCircle_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void button_ImageBuffer001_Register_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void button_ImageBuffer001_Show_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void button_ImageBuffer002_Register_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void button_ImageBuffer002_Show_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void button_001_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void button_002_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void timer_UpdateUI_Tick(System::Object^  sender, System::EventArgs^  e);
	private: System::Void pictureBox_ShowImage_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void numericUpDown_Threshold_ValueChanged(System::Object^  sender, System::EventArgs^  e);
	private: System::Void numericUpDown_Parameter001_ValueChanged(System::Object^  sender, System::EventArgs^  e);
	private: System::Void numericUpDown_Parameter002_ValueChanged(System::Object^  sender, System::EventArgs^  e);
	private: System::Void numericUpDown_Parameter003_ValueChanged(System::Object^  sender, System::EventArgs^  e);
	private: System::Void numericUpDown_MinRadius_ValueChanged(System::Object^  sender, System::EventArgs^  e);
	private: System::Void numericUpDown_MaxRadius_ValueChanged(System::Object^  sender, System::EventArgs^  e);

	private: System::Void NewFunction_001();
	private: System::Void NewFunction_002(System::Object^ sender, EventArgs^ e);
	private: System::Void NewFunction_003(System::Object^ sender);
};

	/// <summary>
	/// ClassOpenCV開發未完成
	/// </summary>
	public class ClassOpenCV
	{
	public:
		Mat OriginMat;
		Mat PresentMat;
		Mat TemporaryMat;
		Mat ImageBuffer[2];

	public:
		int ThresholdValue = 0;
		int HoughCircleParameter001 = 10;
		int HoughCircleParameter002 = 100;
		int HoughCircleParameter003 = 100;
		int HoughCircleMinRadius = 10;
		int HoughCircleMaxRadius = 100;

	public:
		Mat inputMat;
		Mat displayMat;
		Mat *pMat;

	public:
		ClassOpenCV(void)
		{

		}

	public:
		~ClassOpenCV(void)
		{
			
		}

	//刪
	public:
		void WTF(System::Object^ sender)
		{
			std::cout << "What The FUCK!" << endl;
		}
	};

	/// <summary>
	/// ClassSTCCamera未完成
	/// </summary>
	public class ClassSTCCamera
	{
	public:
		bool ErrorFlag = false;
		int ImageNumber = -1;
		CStApiAutoInit objStApiAutoInit;
		CIStDevicePtrArray pIStDeviceList;
		CIStDataStreamPtrArray pIStDataStreamList;
		CIStSystemPtr pIStSystem = CreateIStSystem();
		IStDeviceReleasable *pIStDeviceReleasable = NULL;

	public:
		ClassSTCCamera(void)
		{
			for (;;)
			{
				try
				{
					pIStDeviceReleasable = pIStSystem->CreateFirstIStDevice();
				}
				catch (...)
				{
					if (pIStDeviceList.GetSize() == 0)
					{
						ErrorFlag = true;
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

			if (ErrorFlag)
				std::cout << "攝影機初始化失敗!" << endl;
			else
				std::cout << "攝影機數量 : " << pIStDeviceList.GetSize() << endl;
		}

	public:
		~ClassSTCCamera(void)
		{
			pIStSystem.Move();
		}

	public:
		void Initial(void);
		void Grab(void);
	};
}