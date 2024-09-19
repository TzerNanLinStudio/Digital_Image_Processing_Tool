#pragma comment (lib, "User32.lib")
#include "UI.h"

using namespace UI;
using namespace std;

System::Void UI::MainForm::MainForm_Load(System::Object ^ sender, System::EventArgs ^ e)
{

}

System::Void UI::MainForm::MainForm_Closing(System::Object ^ sender, System::Windows::Forms::FormClosingEventArgs ^ e)
{
	exit(0);
}

System::Void UI::MainForm::btn_Open_Click(System::Object^  sender, System::EventArgs^  e)
{
	System::String^ path;
	OpenFileDialog^ dialog = gcnew OpenFileDialog();
	dialog->Multiselect = false;

	try
	{
		if (dialog->ShowDialog() == System::Windows::Forms::DialogResult::OK)
			path = dialog->InitialDirectory + dialog->FileName;
		std::string std_strfilename = msclr::interop::marshal_as<std::string>(path);

		this->imageCore->SetOriginalImage(std_strfilename);
		this->pictureBox_Image_Displaying->Image = this->imageCore->GetImage("original");
	}
	catch (System::Exception^ ex)
	{
		this->richTextBox_Record->SelectedText = "Open Image Fail.\n";
	}
}

System::Void UI::MainForm::btn_Save_Click(System::Object^  sender, System::EventArgs^  e)
{
	SaveFileDialog^ dialog = gcnew SaveFileDialog();
	dialog->Filter = ".bmp|*.bmp";

	try
	{
		System::Drawing::Bitmap^ bitmap = this->imageCore->GetImage("present");
		if (dialog->ShowDialog() == System::Windows::Forms::DialogResult::OK)
			bitmap->Save(dialog->FileName);
	}
	catch (System::Exception^ ee)
	{
		this->richTextBox_Record->SelectedText = "Save Image Fail.\n";
	}
}

System::Void UI::MainForm::btn_Recover_Click(System::Object^ sender, System::EventArgs^ e)
{
	try
	{
		this->imageCore->Recover();
		this->pictureBox_Image_Displaying->Image = this->imageCore->GetImage("original");
	}
	catch (System::Exception^ ex)
	{
		this->richTextBox_Record->SelectedText = "Show Original Image Fail.\n";
	}
}

System::Void UI::MainForm::btn_Grayscale_Click(System::Object^ sender, System::EventArgs^ e)
{
	try
	{
		this->imageCore->Grayscale();
		this->pictureBox_Image_Displaying->Image = this->imageCore->GetImage("present");
	}
	catch (System::Exception^ ex)
	{
		this->richTextBox_Record->SelectedText = "Grayscale Fail.\n";
	}
}

System::Void UI::MainForm::numericUpDown_Binarize_ValueChanged(System::Object^ sender, System::EventArgs^ e)
{
	try
	{
		this->imageCore->Binarize("test", Convert::ToInt32(this->numericUpDown_Binarize->Value));
		this->pictureBox_Image_Displaying->Image = this->imageCore->GetImage("temporary");
	}
	catch (System::Exception^ ex)
	{
		this->richTextBox_Record->SelectedText = "Binarize Fail.\n";
	}
}

System::Void UI::MainForm::btn_Binarize_Apply_Click(System::Object^ sender, System::EventArgs^ e)
{
	try
	{
		this->imageCore->Binarize("apply", Convert::ToInt32(this->numericUpDown_Binarize->Value));
		this->pictureBox_Image_Displaying->Image = this->imageCore->GetImage("present");
	}
	catch (System::Exception^ ex)
	{
		this->richTextBox_Record->SelectedText = "Apply Fail.\n";
	}
}

System::Void UI::MainForm::btn_Binarize_Cancel_Click(System::Object^ sender, System::EventArgs^ e)
{
	try
	{
		this->pictureBox_Image_Displaying->Image = this->imageCore->GetImage("present");
	}
	catch (System::Exception^ ex)
	{
		this->richTextBox_Record->SelectedText = "Cancel Fail.\n";
	}
}

System::Void UI::MainForm::btn_Morphology_Apply_Click(System::Object^ sender, System::EventArgs^ e)
{
	try
	{
		this->imageCore->Binarize("apply", Convert::ToInt32(this->numericUpDown_Binarize->Value));
		this->pictureBox_Image_Displaying->Image = this->imageCore->GetImage("present");
	}
	catch (System::Exception^ ex)
	{
		this->richTextBox_Record->SelectedText = "Apply Fail.\n";
	}
}