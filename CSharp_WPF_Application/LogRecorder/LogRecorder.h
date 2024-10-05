// LogRecorder.h

#pragma once

using namespace System;
using namespace System::IO;

namespace LOGRECORDER {

	public ref class LogRcdr {
		////////////////////////////////////////////
		//	Properties
		//------------------------------------------
		//	LogPath
		//	LogFilename
		//	Date
		private:
			String			^	LogPath;
			String			^	LogFilename;
			DateTime			LogDate;

			StreamWriter	^	LogFile;


		////////////////////////////////////////////
		//	Method
		//------------------------------------------
		//	Open
		//	Close
		//	Write

		public:
			LogRcdr();
			~LogRcdr();

			void	Open( String ^ Path, String ^ Filename );
			void	Close( void );
			void	Write( String ^ Msg );
	};
}

