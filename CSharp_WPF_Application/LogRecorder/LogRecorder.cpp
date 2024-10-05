// This is the main DLL file.

#include "stdafx.h"

#include "LogRecorder.h"

using namespace System;
using namespace System::IO;
using namespace LOGRECORDER;

//	Constructor
LogRcdr::LogRcdr() {

	this->LogPath		= String::Empty;
	this->LogFilename	= String::Empty;
	this->LogDate		= DateTime::Now;
}

//	Destructor
LogRcdr::~LogRcdr() {

	//	���� SteamWriter �Ŷ�
	if( this->LogFile ) {

		this->LogFile->Close();
		delete	this->LogFile;
	}
}

//	Open
void LogRcdr::Open( String ^ Path, String ^ Filename ) {

	this->LogDate		= DateTime::Now;
	this->LogFilename	= Filename;
	this->LogPath		= Path;
	
	try {

		String	^	StringDate	=	String::Format( "{0:yyyyMMdd}", this->LogDate );

		//	�}���ɮ�
		this->LogFile				= File::AppendText( this->LogPath + "\\" + this->LogFilename + "_" + StringDate + ".Log" );

		//	�]�w�۰ʲM�� - ���Ƽg�J�ɷ|�۰ʱN���F�s�X�����A�~����L���A���M��
		this->LogFile->AutoFlush	= true;

		//	�g�J�ɮ׶}�Үɶ�
		this->LogFile->WriteLine( "=============== LOG START - {0:yyyy}.{0:MM}.{0:dd} {0:HH}:{0:mm}:{0:ss}:{0:ffff} ===============", this->LogDate );

		

	} catch ( Exception	^	e ) {
		System::Windows::Forms::MessageBox::Show( "LogRcdr::Open() - " + e->ToString() );
	}

}

//	Close
void LogRcdr::Close() {

	DateTime		LogCloseDate	= DateTime::Now;

	try {

		//	�g�J�ɮ������ɶ�
		this->LogFile->WriteLine( "===============   LOG END - {0:yyyy}.{0:MM}.{0:dd} {0:HH}:{0:mm}:{0:ss}:{0:ffff} ===============", LogCloseDate );

		this->LogFile->Close();

	} catch ( Exception ^	e ) {
		//	�Y�O�w�Юe�q�����ɡA�����ɮצ��i��|�y���ҥ~�ƥ�
		System::Windows::Forms::MessageBox::Show( "LogRcdr::Close() - " + e->ToString() );
	}

}

//	Write
void LogRcdr::Write( String ^ Msg ) {

	DateTime		MsgWriteDate	= DateTime::Now;
	TimeSpan		s = TimeSpan( MsgWriteDate.Ticks - this->LogDate.Ticks );

	try {
		//	Log�ݨC�Ѧ۰ʧ�s
		//	�P�_��Ƽg�J�ɶ��O�_�M�ɮ׮ɶ��P�@��
		if( s.Days != 0 ) {

			//	��Ƽg�J����M�ɮפ�����P�ѡA�����ɮ׭��s�}��

			this->LogFile->WriteLine("Data write Date is not same as File Date. Close file and Open a new file.");
			this->Close();
			this->Open( this->LogPath, this->LogFilename );
		}

		this->LogFile->WriteLine(
				"{0:yyyy}/{0:MM}/{0:dd}, {0:HH}:{0:mm}:{0:ss}:{0:ffff} - {1}",
				MsgWriteDate,
				Msg
			);

	} catch ( Exception ^	e ) {
		System::Windows::Forms::MessageBox::Show( String::Format( "{0}{1}\n{2}\\{3}",
													"LogRcdr::Write() - ",
													e->Message,
													this->LogPath,
													this->LogFilename ) );
	}
}