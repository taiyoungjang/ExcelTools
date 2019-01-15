TableGenerateCmd 사용방법
Usage: TableGenerateCmd -i 환경설정파일 -c 추출명령 -s 소스폴더 -l 언어[kr|cn] -v 마일스톤버젼 
Option: -i 환경 설정 파일 명
           반드시 실행 파일과 같은 위치에 있어야 한다.
           설정을 안할 경우, TableGenerateCmd.ini 파일을 읽어 온다.
        -c 추출 명령, 명령어는 대소문자를 구분하지 않는다. 옵션을 설정하지 않으면 [all]로 실행된다.
           all: 모든 형식의 파일로 추출한다.
           idl: .IDL, .H, .CPP 파일 형식으로 추출한다.
           c#: .CS 형식의 파일로 추출
           table: .TBL 형식의 파일로 추출
           string: .KOR과 같은 형식으로 추출
           db: .SQL과 같은 형식으로 추출
        -s 소스 폴더. Excel파일이 저장되어 있는 위치 정보
        -l 언어 [kr|cn]
           kr: 한국어
           cn: 중국어
        -v 버전 정보

examlpe:
    TableGenerateCmd.exe
		: 기본 옵션을 사용하여 프로그램을 실행함.
	TableGenerateCmd.exe -i TableGenerateCmd2.ini
		: TableGenerateCmd2.ini 파일 설정을 읽어서 프로그램을 실행함
	TableGenerateCmd -c idl c# table
		: idl, c#, table 형식의 파일만 추출
	TableGenerateCmd -i TableGenerateCmd2.ini -c string db
		: 환경 설정 파일을 읽어와 string, db 형식의 결과물만 추출함.

Usage: TableGenerateCmd -s SourcePath(*.xls) -v 마일스톤버젼 -l [kr|cn]
      => 소스 폴더 정보를 입력할 경우,  $Version 매크로를 사용할 수 있음.
      ex) TableGenerateCmd -s D:\Temp\Table$Version

Usage: TableGenerateCmd -s Table_M12 -l cn -v M12 -c
