#include "MyForm.h"

using namespace UI;

[STAThreadAttribute]

int main(cli::array<System::String ^> ^ args)
{	
	//建立任何程式前，先啟用視覺化效果
	Application::EnableVisualStyles(); //為應用程式啟用視覺化樣式
	Application::SetCompatibleTextRenderingDefault(false); //為部分控制項上定義的 UseCompatibleTextRendering 屬性設定應用程式範圍的預設值。

	//建立主視窗並執行
	MyForm newForm;
	Application::Run(%newForm); //開始執行目前執行緒的標準應用程式訊息迴圈，而不需表單。
}