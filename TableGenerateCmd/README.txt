TableGenerateCmd 사용방법
Usage: TableGenerateCmd -i 환경설정파일 -l 언어[kor|chn] 
Option: -i 환경 설정 파일 명
           반드시 실행 파일과 같은 위치에 있어야 한다.
           설정을 안할 경우, TableGenerateCmd.ini 파일을 읽어 온다.
        -c 추출 명령, 명령어는 대소문자를 구분하지 않는다. 옵션을 설정하지 않으면 [all]로 실행된다.
           all: 모든 형식의 파일로 추출한다.
           c#: .CS 형식의 파일로 추출
           table: .bytes 형식의 파일로 추출
        -s 소스 폴더. Excel파일이 저장되어 있는 위치 정보
        -l 언어 [kr|cn]
           kor: 한국어
           chn: 중국어

