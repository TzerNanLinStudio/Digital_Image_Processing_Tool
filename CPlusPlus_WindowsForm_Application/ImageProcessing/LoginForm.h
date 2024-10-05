#pragma once

namespace UI {

	using namespace System;
	using namespace System::ComponentModel;
	using namespace System::Collections;
	using namespace System::Windows::Forms;
	using namespace System::Data;
	using namespace System::Drawing;

	public enum class LoginState
	{
		NoLogin,
		LoginAsUser,
		LoginAsPM,
		LoginAsSW
	};

	/// <summary>
	/// LoginForm 的摘要
	/// </summary>
	public ref class LoginForm : public System::Windows::Forms::Form
	{
	public:
	public:
		LoginState state = LoginState::LoginAsSW;

	public:
		LoginForm(void)
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
		~LoginForm()
		{
			if (components)
			{
				delete components;
			}
		}

	private: System::Windows::Forms::TextBox^  textBox_Password;
	private: System::Windows::Forms::Button^  button_Login;
	private: System::Windows::Forms::Button^  button_Test001;

	private:
		/// <summary>
		/// 設計工具所需的變數。
		/// </summary>
		System::ComponentModel::Container ^components;

#pragma region Windows Form Designer generated code
		/// <summary>
		/// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
		/// 這個方法的內容。
		/// </summary>
		void InitializeComponent(void)
		{
			this->textBox_Password = (gcnew System::Windows::Forms::TextBox());
			this->button_Login = (gcnew System::Windows::Forms::Button());
			this->button_Test001 = (gcnew System::Windows::Forms::Button());
			this->SuspendLayout();
			// 
			// textBox_Password
			// 
			this->textBox_Password->Location = System::Drawing::Point(289, 67);
			this->textBox_Password->Name = L"textBox_Password";
			this->textBox_Password->Size = System::Drawing::Size(400, 34);
			this->textBox_Password->TabIndex = 0;
			// 
			// button_Login
			// 
			this->button_Login->Location = System::Drawing::Point(247, 172);
			this->button_Login->Name = L"button_Login";
			this->button_Login->Size = System::Drawing::Size(200, 50);
			this->button_Login->TabIndex = 1;
			this->button_Login->Text = L"Login";
			this->button_Login->UseVisualStyleBackColor = true;
			this->button_Login->Click += gcnew System::EventHandler(this, &LoginForm::button_Login_Click);
			// 
			// button_Test001
			// 
			this->button_Test001->Location = System::Drawing::Point(587, 172);
			this->button_Test001->Name = L"button_Test001";
			this->button_Test001->Size = System::Drawing::Size(200, 50);
			this->button_Test001->TabIndex = 2;
			this->button_Test001->Text = L"Test";
			this->button_Test001->UseVisualStyleBackColor = true;
			this->button_Test001->Click += gcnew System::EventHandler(this, &LoginForm::button_Test001_Click);
			// 
			// LoginForm
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(12, 25);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(884, 261);
			this->ControlBox = false;
			this->Controls->Add(this->button_Test001);
			this->Controls->Add(this->button_Login);
			this->Controls->Add(this->textBox_Password);
			this->Font = (gcnew System::Drawing::Font(L"微軟正黑體", 15, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
				static_cast<System::Byte>(136)));
			this->Margin = System::Windows::Forms::Padding(6);
			this->Name = L"LoginForm";
			this->Text = L"LoginForm";
			this->FormClosing += gcnew System::Windows::Forms::FormClosingEventHandler(this, &LoginForm::LoginForm_Closing);
			this->Load += gcnew System::EventHandler(this, &LoginForm::LoginForm_Load);
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion

	private: System::Void LoginForm_Load(System::Object^  sender, System::EventArgs^  e);
	private: System::Void LoginForm_Closing(System::Object^  sender, System::Windows::Forms::FormClosingEventArgs^  e);
	private: System::Void button_Login_Click(System::Object^  sender, System::EventArgs^  e);
	private: System::Void button_Test001_Click(System::Object^  sender, System::EventArgs^  e);
};
}
