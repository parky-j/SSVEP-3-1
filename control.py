# mave 기계 데이터 저장경로
LEFT = "Fp1_FFT.txt" 
RIGHT = "Fp2_FFT.txt"
SAVEPATH = r'C:\MAVE_RawData\temp.txt'
BASERAWDATAPATH = r'C:\MAVE_RawData\\'
MAVEHZ = 0.2 #mave 주파수 해상도 설정에 맞춰 설정 이경우 0.2hz
import csv
import datetime
import numpy
from os import write
# mave 기계의 파일 저장형식 
# 0.0hz 0.2hz 0.3hz
#   1     2     3 
#   4     5     6 
def reading(address):
    f = open(address,"r")
    reader = csv.reader(f)
    data = []
    for line in reader:
        data.append(line[0].split('\t'))
    return data
def readsave(address):
    f = open(address,"r")
    reader = csv.reader(f)
    for line in reader:
        return line

def save(address,data):
    f = open(address,'w')
    writer = csv.writer(f)
    writer.writerow(data)

def txttobraincsv(braintxt):
    return numpy.array(list(map(float,braintxt)))

def subtime(time,subsecond,subminute,subhour):
    hour = time.hour - subhour
    if hour < 0:
        hour += 24
    minute = time.minute - subminute
    if minute < 0:
        hour -= 1
        minute += 60
    second = time.second - subsecond
    if second < 0:
        minute -= 1
        second = time.second + 60
    return datetime.time(hour,minute,second)
        
def extractdata(brainwavearray,time,timebetweensecond):
    endt = time
    startt = subtime(time,0,0,timebetweensecond)
    returndata = []
    returndata.append(brainwavearray[0])
    for count in range(1,brainwavearray.__len__()):
        if(startt <= datetime.datetime.strptime(brainwavearray[count][0][3:].split('.')[0],"%H:%M:%S").time() <= endt):
            returndata.append(brainwavearray[count])
    return returndata

def concat(factor1,factor2):
    return factor1 + factor2[1:]

def sub(factor1,factor2):
    return factor1 - factor2

def average(factor):
    data = numpy.array(list(map(float,factor[1][1:])))
    for i in range(2,factor.__len__()):
        data = data + numpy.array(list(map(float,factor[i][1:])))
    return data/factor.__len__()

def extractpeak(braindata,hz,bandwidth):
    start = int(hz * 5 - bandwidth / MAVEHZ)
    end = int(hz * 5 + bandwidth / MAVEHZ)
    return max(braindata[start:end])


