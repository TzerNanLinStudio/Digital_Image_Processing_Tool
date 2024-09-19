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
#include <msclr/marshal_cppstd.h> 
#include <msclr/marshal.h>
#include "Data.h"

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
	using namespace cv;

	[System::Runtime::InteropServices::ComVisible(false)]

	/// <summary>
	/// 
	/// </summary>
	public ref class MainForm : public System::Windows::Forms::Form
	{
	private:
		ImageCore* imageCore = new ImageCore();

	public:
		MainForm(void)
		{
			InitializeComponent();		
		}
		
	protected:
		~MainForm()
		{
			if (components)
			{
				delete components;
			}
		}

	private: System::Windows::Forms::Button^ btn_Grayscale;
	private: System::Windows::Forms::Button^ btn_Binarize_Cancel;
	private: System::Windows::Forms::GroupBox^ groupBox_OtherFunctionality;
	private: System::Windows::Forms::GroupBox^ groupBox_Image;
	private: System::Windows::Forms::GroupBox^ groupBox_Message;
	private: System::Windows::Forms::GroupBox^  groupBox_Morphology;
	private: System::Windows::Forms::Button^  btn_Save;
	private: System::Windows::Forms::NumericUpDown^ numericUpDown_Binarize;
	private: System::Windows::Forms::Button^ btn_Binarize_Apply;
	private: System::Windows::Forms::RichTextBox^  richTextBox_Record;
	private: System::Windows::Forms::GroupBox^ groupBox_Binarize;
	private: System::Windows::Forms::PictureBox^  pictureBox_Image_Displaying;
	private: System::Windows::Forms::Button^  btn_Open;
	private: System::Windows::Forms::OpenFileDialog^  openFileDialog1;
	private: System::Windows::Forms::ImageList^  imageList1;
	private: System::Windows::Forms::Button^ btn_Recover;
	private: System::Windows::Forms::Button^ btn_Morphology_Apply;
	private: System::ComponentModel::IContainer^ components;

	private:
		#pragma region Windows Form Designer generated code
		void InitializeComponent(void)
		{
			this->components = (gcnew System::ComponentModel::Container());
			this->pictureBox_Image_Displaying = (gcnew System::Windows::Forms::PictureBox());
			this->btn_Open = (gcnew System::Windows::Forms::Button());
			this->openFileDialog1 = (gcnew System::Windows::Forms::OpenFileDialog());
			this->imageList1 = (gcnew System::Windows::Forms::ImageList(this->components));
			this->btn_Recover = (gcnew System::Windows::Forms::Button());
			this->btn_Binarize_Apply = (gcnew System::Windows::Forms::Button());
			this->numericUpDown_Binarize = (gcnew System::Windows::Forms::NumericUpDown());
			this->btn_Save = (gcnew System::Windows::Forms::Button());
			this->richTextBox_Record = (gcnew System::Windows::Forms::RichTextBox());
			this->groupBox_Binarize = (gcnew System::Windows::Forms::GroupBox());
			this->btn_Binarize_Cancel = (gcnew System::Windows::Forms::Button());
			this->btn_Grayscale = (gcnew System::Windows::Forms::Button());
			this->groupBox_Morphology = (gcnew System::Windows::Forms::GroupBox());
			this->groupBox_Image = (gcnew System::Windows::Forms::GroupBox());
			this->groupBox_Message = (gcnew System::Windows::Forms::GroupBox());
			this->groupBox_OtherFunctionality = (gcnew System::Windows::Forms::GroupBox());
			this->btn_Morphology_Apply = (gcnew System::Windows::Forms::Button());
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^>(this->pictureBox_Image_Displaying))->BeginInit();
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^>(this->numericUpDown_Binarize))->BeginInit();
			this->groupBox_Binarize->SuspendLayout();
			this->groupBox_Morphology->SuspendLayout();
			this->groupBox_Image->SuspendLayout();
			this->groupBox_Message->SuspendLayout();
			this->SuspendLayout();
			// 
			// pictureBox_Image_Displaying
			// 
			this->pictureBox_Image_Displaying->Location = System::Drawing::Point(6, 21);
			this->pictureBox_Image_Displaying->Name = L"pictureBox_Image_Displaying";
			this->pictureBox_Image_Displaying->Size = System::Drawing::Size(600, 450);
			this->pictureBox_Image_Displaying->SizeMode = System::Windows::Forms::PictureBoxSizeMode::Zoom;
			this->pictureBox_Image_Displaying->TabIndex = 3;
			this->pictureBox_Image_Displaying->TabStop = false;
			// 
			// btn_Open
			// 
			this->btn_Open->Location = System::Drawing::Point(643, 23);
			this->btn_Open->Name = L"btn_Open";
			this->btn_Open->Size = System::Drawing::Size(150, 30);
			this->btn_Open->TabIndex = 6;
			this->btn_Open->Text = L"Open";
			this->btn_Open->UseVisualStyleBackColor = true;
			this->btn_Open->Click += gcnew System::EventHandler(this, &MainForm::btn_Open_Click);
			// 
			// openFileDialog1
			// 
			this->openFileDialog1->FileName = L"openFileDialog1";
			// 
			// imageList1
			// 
			this->imageList1->ColorDepth = System::Windows::Forms::ColorDepth::Depth8Bit;
			this->imageList1->ImageSize = System::Drawing::Size(16, 16);
			this->imageList1->TransparentColor = System::Drawing::Color::Transparent;
			// 
			// btn_Recover
			// 
			this->btn_Recover->Location = System::Drawing::Point(643, 95);
			this->btn_Recover->Name = L"btn_Recover";
			this->btn_Recover->Size = System::Drawing::Size(150, 30);
			this->btn_Recover->TabIndex = 17;
			this->btn_Recover->Text = L"Recover";
			this->btn_Recover->UseVisualStyleBackColor = true;
			this->btn_Recover->Click += gcnew System::EventHandler(this, &MainForm::btn_Recover_Click);
			// 
			// btn_Binarize_Apply
			// 
			this->btn_Binarize_Apply->Location = System::Drawing::Point(15, 59);
			this->btn_Binarize_Apply->Name = L"btn_Binarize_Apply";
			this->btn_Binarize_Apply->Size = System::Drawing::Size(60, 30);
			this->btn_Binarize_Apply->TabIndex = 9;
			this->btn_Binarize_Apply->Text = L"Apply";
			this->btn_Binarize_Apply->UseVisualStyleBackColor = true;
			this->btn_Binarize_Apply->Click += gcnew System::EventHandler(this, &MainForm::btn_Binarize_Apply_Click);
			// 
			// numericUpDown_Binarize
			// 
			this->numericUpDown_Binarize->Location = System::Drawing::Point(15, 25);
			this->numericUpDown_Binarize->Maximum = System::Decimal(gcnew cli::array< System::Int32 >(4) { 255, 0, 0, 0 });
			this->numericUpDown_Binarize->Name = L"numericUpDown_Binarize";
			this->numericUpDown_Binarize->Size = System::Drawing::Size(120, 22);
			this->numericUpDown_Binarize->TabIndex = 27;
			this->numericUpDown_Binarize->ValueChanged += gcnew System::EventHandler(this, &MainForm::numericUpDown_Binarize_ValueChanged);
			// 
			// btn_Save
			// 
			this->btn_Save->Location = System::Drawing::Point(643, 59);
			this->btn_Save->Name = L"btn_Save";
			this->btn_Save->Size = System::Drawing::Size(150, 30);
			this->btn_Save->TabIndex = 24;
			this->btn_Save->Text = L"Save";
			this->btn_Save->UseVisualStyleBackColor = true;
			this->btn_Save->Click += gcnew System::EventHandler(this, &MainForm::btn_Save_Click);
			// 
			// richTextBox_Record
			// 
			this->richTextBox_Record->BackColor = System::Drawing::SystemColors::Window;
			this->richTextBox_Record->ForeColor = System::Drawing::SystemColors::WindowText;
			this->richTextBox_Record->Location = System::Drawing::Point(6, 21);
			this->richTextBox_Record->Name = L"richTextBox_Record";
			this->richTextBox_Record->ReadOnly = true;
			this->richTextBox_Record->Size = System::Drawing::Size(600, 60);
			this->richTextBox_Record->TabIndex = 28;
			this->richTextBox_Record->Text = L"";
			// 
			// groupBox_Binarize
			// 
			this->groupBox_Binarize->Controls->Add(this->btn_Binarize_Cancel);
			this->groupBox_Binarize->Controls->Add(this->numericUpDown_Binarize);
			this->groupBox_Binarize->Controls->Add(this->btn_Binarize_Apply);
			this->groupBox_Binarize->Location = System::Drawing::Point(648, 167);
			this->groupBox_Binarize->Name = L"groupBox_Binarize";
			this->groupBox_Binarize->Size = System::Drawing::Size(145, 102);
			this->groupBox_Binarize->TabIndex = 35;
			this->groupBox_Binarize->TabStop = false;
			this->groupBox_Binarize->Text = L"Binarize";
			// 
			// btn_Binarize_Cancel
			// 
			this->btn_Binarize_Cancel->Location = System::Drawing::Point(75, 59);
			this->btn_Binarize_Cancel->Name = L"btn_Binarize_Cancel";
			this->btn_Binarize_Cancel->Size = System::Drawing::Size(60, 30);
			this->btn_Binarize_Cancel->TabIndex = 28;
			this->btn_Binarize_Cancel->Text = L"Cancel";
			this->btn_Binarize_Cancel->UseVisualStyleBackColor = true;
			this->btn_Binarize_Cancel->Click += gcnew System::EventHandler(this, &MainForm::btn_Binarize_Cancel_Click);
			// 
			// btn_Grayscale
			// 
			this->btn_Grayscale->Location = System::Drawing::Point(643, 131);
			this->btn_Grayscale->Name = L"btn_Grayscale";
			this->btn_Grayscale->Size = System::Drawing::Size(150, 30);
			this->btn_Grayscale->TabIndex = 46;
			this->btn_Grayscale->Text = L"Grayscale";
			this->btn_Grayscale->UseVisualStyleBackColor = true;
			this->btn_Grayscale->Click += gcnew System::EventHandler(this, &MainForm::btn_Grayscale_Click);
			// 
			// groupBox_Morphology
			// 
			this->groupBox_Morphology->Controls->Add(this->btn_Morphology_Apply);
			this->groupBox_Morphology->Location = System::Drawing::Point(648, 275);
			this->groupBox_Morphology->Name = L"groupBox_Morphology";
			this->groupBox_Morphology->Size = System::Drawing::Size(145, 120);
			this->groupBox_Morphology->TabIndex = 38;
			this->groupBox_Morphology->TabStop = false;
			this->groupBox_Morphology->Text = L"Morphology";
			// 
			// groupBox_Image
			// 
			this->groupBox_Image->Controls->Add(this->pictureBox_Image_Displaying);
			this->groupBox_Image->Location = System::Drawing::Point(12, 12);
			this->groupBox_Image->Name = L"groupBox_Image";
			this->groupBox_Image->Size = System::Drawing::Size(614, 481);
			this->groupBox_Image->TabIndex = 47;
			this->groupBox_Image->TabStop = false;
			this->groupBox_Image->Text = L"Image";
			// 
			// groupBox_Message
			// 
			this->groupBox_Message->Controls->Add(this->richTextBox_Record);
			this->groupBox_Message->Location = System::Drawing::Point(12, 499);
			this->groupBox_Message->Name = L"groupBox_Message";
			this->groupBox_Message->Size = System::Drawing::Size(614, 91);
			this->groupBox_Message->TabIndex = 48;
			this->groupBox_Message->TabStop = false;
			this->groupBox_Message->Text = L"Message";
			// 
			// groupBox_OtherFunctionality
			// 
			this->groupBox_OtherFunctionality->Location = System::Drawing::Point(648, 402);
			this->groupBox_OtherFunctionality->Name = L"groupBox_OtherFunctionality";
			this->groupBox_OtherFunctionality->Size = System::Drawing::Size(145, 187);
			this->groupBox_OtherFunctionality->TabIndex = 49;
			this->groupBox_OtherFunctionality->TabStop = false;
			this->groupBox_OtherFunctionality->Text = L"Other Functionality";
			// 
			// btn_Morphology_Apply
			// 
			this->btn_Morphology_Apply->Location = System::Drawing::Point(15, 73);
			this->btn_Morphology_Apply->Name = L"btn_Morphology_Apply";
			this->btn_Morphology_Apply->Size = System::Drawing::Size(60, 30);
			this->btn_Morphology_Apply->TabIndex = 29;
			this->btn_Morphology_Apply->Text = L"Apply";
			this->btn_Morphology_Apply->UseVisualStyleBackColor = true;
			this->btn_Morphology_Apply->Click += gcnew System::EventHandler(this, &MainForm::btn_Morphology_Apply_Click);
			// 
			// MainForm
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 12);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(804, 601);
			this->Controls->Add(this->groupBox_OtherFunctionality);
			this->Controls->Add(this->groupBox_Message);
			this->Controls->Add(this->groupBox_Image);
			this->Controls->Add(this->groupBox_Morphology);
			this->Controls->Add(this->btn_Grayscale);
			this->Controls->Add(this->groupBox_Binarize);
			this->Controls->Add(this->btn_Save);
			this->Controls->Add(this->btn_Recover);
			this->Controls->Add(this->btn_Open);
			this->Name = L"MainForm";
			this->Text = L"Demo";
			this->FormClosing += gcnew System::Windows::Forms::FormClosingEventHandler(this, &MainForm::MainForm_Closing);
			this->Load += gcnew System::EventHandler(this, &MainForm::MainForm_Load);
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^>(this->pictureBox_Image_Displaying))->EndInit();
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^>(this->numericUpDown_Binarize))->EndInit();
			this->groupBox_Binarize->ResumeLayout(false);
			this->groupBox_Morphology->ResumeLayout(false);
			this->groupBox_Image->ResumeLayout(false);
			this->groupBox_Message->ResumeLayout(false);
			this->ResumeLayout(false);

		}
		#pragma endregion

		private: System::Void MainForm_Load(System::Object^  sender, System::EventArgs^  e);
		private: System::Void MainForm_Closing(System::Object^  sender, System::Windows::Forms::FormClosingEventArgs ^ e);
		private: System::Void btn_Open_Click(System::Object^  sender, System::EventArgs^  e);
		private: System::Void btn_Save_Click(System::Object^  sender, System::EventArgs^  e);
		private: System::Void btn_Recover_Click(System::Object^  sender, System::EventArgs^  e);
		private: System::Void btn_Grayscale_Click(System::Object^  sender, System::EventArgs^  e);
		private: System::Void btn_Binarize_Apply_Click(System::Object^  sender, System::EventArgs^  e);
		private: System::Void btn_Binarize_Cancel_Click(System::Object^ sender, System::EventArgs^ e);
		private: System::Void numericUpDown_Binarize_ValueChanged(System::Object^  sender, System::EventArgs^  e);
		private: System::Void btn_Morphology_Apply_Click(System::Object^ sender, System::EventArgs^ e);
};
}