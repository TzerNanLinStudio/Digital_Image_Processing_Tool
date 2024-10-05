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

	//	釋放 SteamWriter 空間
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

		//	開啟檔案
		this->LogFile				= File::AppendText( this->LogPath + "\\" + this->LogFilename + "_" + StringDate + ".Log" );

		//	設定自動清除 - 當資料寫入時會自動將除了編碼器狀態外的其他狀態都清除
		this->LogFile->AutoFlush	= true;

		//	寫入檔案開啟時間
		this->LogFile->WriteLine( "=============== LOG START - {0:yyyy}.{0:MM}.{0:dd} {0:HH}:{0:mm}:{0:ss}:{0:ffff} ===============", this->LogDate );

		

	} catch ( Exception	^	e ) {
		System::Windows::Forms::MessageBox::Show( "LogRcdr::Open() - " + e->ToString() );
	}

}

//	Close
void LogRcdr::Close() {

	DateTime		LogCloseDate	= DateTime::Now;

	try {

		//	寫入檔案關閉時間
		this->LogFile->WriteLine( "===============   LOG END - {0:yyyy}.{0:MM}.{0:dd} {0:HH}:{0:mm}:{0:ss}:{0:ffff} ===============", LogCloseDate );

		this->LogFile->Close();

	} catch ( Exception ^	e ) {
		//	若是硬碟容量不夠時，關閉檔案有可能會造成例外事件
		System::Windows::Forms::MessageBox::Show( "LogRcdr::Close() - " + e->ToString() );
	}

}

//	Write
void LogRcdr::Write( String ^ Msg ) {

	DateTime		MsgWriteDate	= DateTime::Now;
	TimeSpan		s = TimeSpan( MsgWriteDate.Ticks - this->LogDate.Ticks );

	try {
		//	Log需每天自動更新
		//	判斷資料寫入時間是否和檔案時間同一天
		if( s.Days != 0 ) {

			//	資料寫入日期和檔案日期不同天，關閉檔案重新開啟

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