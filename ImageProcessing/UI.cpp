#include "MyForm.h"

using namespace UI;

[STAThreadAttribute]

int main(cli::array<System::String ^> ^ args)
{	
	//�إߥ���{���e�A���ҥε�ı�ƮĪG
	Application::EnableVisualStyles(); //�����ε{���ҥε�ı�Ƽ˦�
	Application::SetCompatibleTextRenderingDefault(false); //����������W�w�q�� UseCompatibleTextRendering �ݩʳ]�w���ε{���d�򪺹w�]�ȡC

	//�إߥD�����ð���
	MyForm newForm;
	Application::Run(%newForm); //�}�l����ثe��������з����ε{���T���j��A�Ӥ��ݪ��C
}