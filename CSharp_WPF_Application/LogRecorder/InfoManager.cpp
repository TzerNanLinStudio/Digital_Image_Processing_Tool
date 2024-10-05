
#include "stdafx.h"

#include "InfoManager.h"

using namespace System;
using namespace System::IO;

using namespace LOGRECORDER;

//	Constructor
InfoMgr::InfoMgr( String ^ DirGenLog, String ^ DirWarningLog, String ^ DirErrLog, String ^ DirDebugLog ) {

	this->rtB_GenLog		= nullptr;
	this->rtB_WarningLog	= nullptr;
	this->rtB_ErrLog		= nullptr;
	this->rtB_DebugLog		= nullptr;


	DirectoryInfo ^ TmpDir_GenLog = gcnew DirectoryInfo( DirGenLog );
	DirectoryInfo ^ TmpDir_WarningLog = gcnew DirectoryInfo( DirWarningLog );
	DirectoryInfo ^ TmpDir_ErrLog = gcnew DirectoryInfo( DirErrLog );
	DirectoryInfo ^ TmpDir_DebugLog = gcnew DirectoryInfo( DirDebugLog );

	try {

		if( !TmpDir_GenLog->Exists ) {
			TmpDir_GenLog->Create();
			System::Threading::Thread::Sleep( 20 );		//	Wait OS Create Directory
		}
		this->GenLog		= gcnew LogRcdr();
		this->GenLog->Open( DirGenLog, "GeneralLog" );

		if( !TmpDir_WarningLog->Exists ) {
			TmpDir_WarningLog->Create();
			System::Threading::Thread::Sleep( 20 );		//	Wait OS Create Directory
		}
		this->WarningLog	= gcnew LogRcdr();
		this->WarningLog->Open( DirWarningLog, "WarningLog" );

		if( !TmpDir_ErrLog->Exists ) {
			TmpDir_ErrLog->Create();
			System::Threading::Thread::Sleep( 20 );		//	Wait OS Create Directory
		}
		this->ErrLog		= gcnew LogRcdr();
		this->ErrLog->Open( DirErrLog, "ErrorLog" );

		if( !TmpDir_DebugLog->Exists ) {
			TmpDir_DebugLog->Create();
			System::Threading::Thread::Sleep( 20 );		//	Wait OS Create Directory
		}
		this->DebugLog		= gcnew LogRcdr();
		this->DebugLog->Open( DirDebugLog, "DebugLog" );

	} catch ( Exception	^	e ) {
		throw	e;
	}

}

//	Destructor
InfoMgr::~InfoMgr() {

	this->GenLog->Close();
	this->WarningLog->Close();
	this->ErrLog->Close();
	this->DebugLog->Close();

}

void InfoMgr::SetGenLogRTB( System::Windows::Forms::RichTextBox ^ Obj_rtB ) {
	if( Obj_rtB == nullptr ) {
		throw	gcnew System::Exception( "RichTextBox is null." );
	}

	this->rtB_GenLog	= Obj_rtB;
}

void InfoMgr::SetWarningLogRTB( System::Windows::Forms::RichTextBox ^ Obj_rtB ) {
	if( Obj_rtB == nullptr ) {
		throw	gcnew System::Exception( "RichTextBox is null." );
	}

	this->rtB_WarningLog	= Obj_rtB;
}

void InfoMgr::SetErrLogRTB( System::Windows::Forms::RichTextBox ^ Obj_rtB ) {
	if( Obj_rtB == nullptr ) {
		throw	gcnew System::Exception( "RichTextBox is null." );
	}

	this->rtB_ErrLog	= Obj_rtB;
}

void InfoMgr::SetDebugLogRTB( System::Windows::Forms::RichTextBox ^ Obj_rtB ) {
	if( Obj_rtB == nullptr ) {
		throw	gcnew System::Exception( "RichTextBox is null." );
	}

	this->rtB_DebugLog	= Obj_rtB;
	this->rtB_DebugLog->SelectionColor		= System::Drawing::Color::Magenta;
}


void InfoMgr::MsgGenLog( String ^ Msg ) {

	//	�g�JLog����
	this->GenLog->Write( Msg );	

	//	�T�{�O�_�������q�X
	if( this->rtB_GenLog	!= nullptr ) {

		this->MsgToRTB( this->rtB_GenLog, System::Drawing::Color::Black, Msg );

	}

}

void InfoMgr::MsgHLLog( String ^ Msg ) {

	//	�g�JLog����
	this->GenLog->Write( Msg );

	//	�T�{�O�_�������q�X
	if( this->rtB_GenLog	!= nullptr ) {

		this->MsgToRTB( this->rtB_GenLog, System::Drawing::Color::DarkBlue, Msg );
		
	}
}

void InfoMgr::MsgRmtCtrlLog( String ^ Msg ) {

	//	�g�JLog����
	this->GenLog->Write( Msg );

	//	�T�{�O�_�������q�X
	if( this->rtB_GenLog	!= nullptr ) {

		this->MsgToRTB( this->rtB_GenLog, System::Drawing::Color::DarkGreen, Msg );
		
	}
}

void InfoMgr::MsgUILog( String ^ Msg ) {

	//	�g�JLog����
	this->GenLog->Write( Msg );

	//	�T�{�O�_�������q�X
	if( this->rtB_GenLog	!= nullptr ) {

		this->MsgToRTB( this->rtB_GenLog, System::Drawing::Color::DarkCyan, Msg );
		
	}
}

void InfoMgr::MsgWarning( String ^ Msg ) {

	//	�g�JLog����
	this->GenLog->Write( Msg );
	this->WarningLog->Write( Msg );

	//	�T�{�O�_�����q�����n�q�X
	if( this->rtB_GenLog		!= nullptr ) {

		this->MsgToRTB( this->rtB_GenLog, System::Drawing::Color::Orange, Msg );

	}

	//	�T�{�O�_��ĵ�i�����n�q�X
	if( this->rtB_WarningLog	!= nullptr ) {

		this->MsgToRTB( this->rtB_WarningLog, System::Drawing::Color::Orange, Msg );

	}
}

void InfoMgr::MsgError( String ^ Msg ) {

	//	�g�JLog����
	this->GenLog->Write( Msg );
	this->ErrLog->Write( Msg );

	//	�T�{�O�_�����q�����n�q�X
	if( this->rtB_GenLog		!= nullptr ) {

		this->MsgToRTB( this->rtB_GenLog, System::Drawing::Color::Red, Msg );

	}

	//	�T�{�O�_�����~�����n�q�X
	if( this->rtB_ErrLog	!= nullptr ) {

		this->MsgToRTB( this->rtB_ErrLog, System::Drawing::Color::Red, Msg );

	}

}

void InfoMgr::MsgDebug( String ^ Msg ) {

	//	�g�JLog����
	this->DebugLog->Write( Msg );

	//	�T�{�O�_�����������n�q�X
	if( this->rtB_DebugLog	!= nullptr ) {

		this->MsgToRTB( this->rtB_DebugLog, System::Drawing::Color::Magenta, Msg );

	}

}


void InfoMgr::MsgToRTB( System::Windows::Forms::RichTextBox ^ o_rtB, System::Drawing::Color color, String ^ Msg ) {

	if( o_rtB->InvokeRequired ) {

		MsgDelegate	^	action = gcnew MsgDelegate( this, &InfoMgr::MsgToRTB );

		o_rtB->BeginInvoke( action, o_rtB, color, Msg );

	} else {

		//	����
		if( o_rtB->Lines->Length > 0 )
			o_rtB->AppendText( "\n" );

		//	��J�T��

		String	^		AppendText;
		DateTime		AppendTextDate	= DateTime::Now;

		AppendText	= String::Format( "{0:yyyy}/{0:MM}/{0:dd}, {0:HH}:{0:mm}:{0:ss}:{0:ffff} > {1}",
										AppendTextDate,
										Msg );

		o_rtB->SelectionColor		= color;
		o_rtB->AppendText( AppendText );


		//	�Y�O�W�L�̤j�W���A�R���Ĥ@��A��L�O�d
		if( o_rtB->Lines->Length > this->MaxLogLine ) {

			 String		^	rtfString;
			 
			 o_rtB->Select( o_rtB->GetFirstCharIndexFromLine( 1 ), o_rtB->TextLength );

			 rtfString	= o_rtB->SelectedRtf;

			 o_rtB->Rtf	= rtfString;

		 }

		//	���в���̫�
		o_rtB->SelectionStart = o_rtB->TextLength;
		o_rtB->ScrollToCaret();

	}

}