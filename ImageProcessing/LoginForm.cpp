#include "LoginForm.h"

using namespace UI;

System::Void UI::LoginForm::LoginForm_Load(System::Object^  sender, System::EventArgs^  e)
{
	Console::WriteLine("Hello LoginForm!");
}

System::Void UI::LoginForm::LoginForm_Closing(System::Object^  sender, System::Windows::Forms::FormClosingEventArgs^  e)
{
	
	Console::WriteLine("Bye LoginForm!");
}

System::Void UI::LoginForm::button_Login_Click(System::Object^  sender, System::EventArgs^  e)
{
	this->Hide();
	this->Close();
}

System::Void UI::LoginForm::button_Test001_Click(System::Object^  sender, System::EventArgs^  e)
{
	switch (this->state)
	{
	case LoginState::LoginAsSW:
		MessageBox::Show("SW Login.");
		break;

	case LoginState::LoginAsPM:
		MessageBox::Show("PM Login.");
		break;

	case LoginState::LoginAsUser:
		MessageBox::Show("User Login.");
		break;
	}
}
