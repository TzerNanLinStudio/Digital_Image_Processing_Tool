#pragma once

#include "LogRecorder.h"

using namespace System;

namespace LOGRECORDER {

	public delegate void MsgDelegate( System::Windows::Forms::RichTextBox ^, System::Drawing::Color, String ^ );

	public ref class InfoMgr {

		////////////////////////////////////////////
		//	Properties
		//------------------------------------------
		private:
			const static int		MaxLogLine	= 1000;
		private:
			LogRcdr				^	GenLog;
			LogRcdr				^	WarningLog;
			LogRcdr				^	ErrLog;
			LogRcdr				^	DebugLog;

			System::Windows::Forms::RichTextBox		^	rtB_GenLog;
			System::Windows::Forms::RichTextBox		^	rtB_WarningLog;
			System::Windows::Forms::RichTextBox		^	rtB_ErrLog;
			System::Windows::Forms::RichTextBox		^	rtB_DebugLog;

		////////////////////////////////////////////
		//	Method
		//------------------------------------------
		public:
			InfoMgr( String ^ DirGenLog, String ^ DirWarningLog, String ^ DirErrLog, String ^ DirDebugLog );
			~InfoMgr();

			void	SetGenLogRTB( System::Windows::Forms::RichTextBox ^ Obj_rtB );
			void	SetWarningLogRTB( System::Windows::Forms::RichTextBox ^ Obj_rtB );
			void	SetErrLogRTB( System::Windows::Forms::RichTextBox ^ Obj_rtB );
			void	SetDebugLogRTB( System::Windows::Forms::RichTextBox ^ Obj_rtB );

													//	Color			Log		Warn	Err		Debug
			void	MsgGenLog( String ^ Msg );		//	default			��
			void	MsgHLLog( String ^ Msg );		//	DarkBlue		��
			void	MsgRmtCtrlLog( String ^ Msg );	//	DarkGreen		��
			void	MsgUILog( String ^ Msg );		//	DarkCyan		��
			void	MsgWarning( String ^ Msg );		//	Yellow			��		��
			void	MsgError( String ^ Msg );		//	Red				��				��
			void	MsgDebug( String ^ Msg );		//	Magenta			��						��

		private:
			void	MsgToRTB( System::Windows::Forms::RichTextBox ^ o_rtB, System::Drawing::Color color, String ^ Msg );

	};
}