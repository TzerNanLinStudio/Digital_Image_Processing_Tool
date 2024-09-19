#include "UI.h"

using namespace System;
using namespace System::Windows::Forms;
using namespace UI;

[STAThreadAttribute]

int main(array<System::String ^> ^ args)
{
	Application::EnableVisualStyles(); 
	Application::SetCompatibleTextRenderingDefault(false); 

	MainForm newForm;
	Application::Run(%newForm); 
}
