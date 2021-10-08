#이전에 실행된 프로그램이 만든 평균값과 현재 뇌파를 측정해서 만든 5초 평균값을 뺀다(대부분의 오차를 제거)
#전두옆 특성상 다른옆으로부터 오는 모든값이 합쳐져있다고 볼수있고 각각의 옆에서 오는 값들의 가중치도 알수 없기에
#자극 유무의 차이를 통해 통제변수를 모두 제거한다
#다른 엽으로부터 오는 값이 거의다 제거되었다고 가정하고 강한 값이 측정되는 뇌파를 뽑아낸다
import control
import datetime
import os
def preprocessing():
    for root,dirs,files in os.walk(control.BASERAWDATAPATH):
        directorysource = dirs
        break
    time = datetime.datetime.now()
    #========테스트용 삭제
    #time = datetime.time(5,38,59)
    #========
    brainwavearrayl = control.reading(control.BASERAWDATAPATH + directorysource[0] + "\\" + control.LEFT) #뇌파 데이터 읽어오기
    brainwavearrayr = control.reading(control.BASERAWDATAPATH + directorysource[0] + "\\" + control.RIGHT)
    brainwavearrayl = control.extractdata(brainwavearrayl,time,5) #프로그램이 실행되기 5초전의 데이터부터 현재 데이터 까지 추출 
    brainwavearrayr = control.extractdata(brainwavearrayr,time,5)
    brainwavearray = control.concat(brainwavearrayl,brainwavearrayr) #이어붙이기
    brainwavearray = control.average(brainwavearray) #평균
    normalbrainwave = control.readsave(control.SAVEPATH) #평상시 데이터 가져오기
    normalbrainwave = control.txttobraincsv(normalbrainwave)
    return control.sub(normalbrainwave,brainwavearray) #자극 데이터와 평상시 데이터 차이

def hztoresult(hz13,hz26,hz39,hz23,hz46,hz35): #뇌파 처리 최종 결과
    data = [hz13,hz26,hz39,hz23,hz46,hz35]
    return '13' if (hz13 or hz26 or hz39) == max(data) else ('23' if hz23==max(data) else '35')

if __name__ == '__main__':
    braindata = preprocessing()
    hz13 = control.extractpeak(braindata,13,control.MAVEHZ * 2) #13hz 와 주변 2개의 데이터에서 가장 강한 주파수 추출
    hz26 = control.extractpeak(braindata,13,control.MAVEHZ * 2)
    hz39 = control.extractpeak(braindata,39,control.MAVEHZ * 2)
    hz23 = control.extractpeak(braindata,23,control.MAVEHZ * 2)
    hz46 = control.extractpeak(braindata,46,control.MAVEHZ * 2)
    hz35 = control.extractpeak(braindata,35,control.MAVEHZ * 2)
    print(hztoresult(hz13,hz26,hz39,hz23,hz46,hz35),end='') #결과 출력
