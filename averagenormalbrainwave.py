#시작할때 이 프로그램이 실행되며 실행된 시간의 시점부터 5초동안 자극이 없는 평상시의 뇌파를 측정하고 평균값을 구한다
import control
import datetime
import os
time = datetime.datetime.now().time()
for root,dirs,files in os.walk(control.BASERAWDATAPATH):
    directorysource = dirs
    break
#========테스트용삭제
#time = datetime.time(5,38,45)
#========
brainwavearrayl = control.reading(control.BASERAWDATAPATH + directorysource[0] + "\\" + control.LEFT) #뇌파 데이터 읽어오기
brainwavearrayr = control.reading(control.BASERAWDATAPATH + directorysource[0] + "\\" + control.RIGHT)
time = control.subtime(time,0,0,1) #자극을 주기전 인식으로 인한 변수제거를 위해 1초전의 시간까지 측정한다
brainwavearrayl = control.extractdata(brainwavearrayl,time,5) #프로그램이 실행되기 5초전의 데이터부터 현재 데이터 까지 추출 
brainwavearrayr = control.extractdata(brainwavearrayr,time,5)
brainwavearray = control.concat(brainwavearrayl,brainwavearrayr) #좌뇌 우뇌 데이터 이어붙이기
brainwavearray = control.average(brainwavearray) #평균값 출력
if not os.path.dirname(control.SAVEPATH):
    os.mkdir(control.SAVEPATH)
control.save(control.SAVEPATH,brainwavearray)