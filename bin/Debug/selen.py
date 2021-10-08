from selenium import webdriver
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.common.alert import Alert
from selenium.webdriver.common.action_chains import ActionChains

import time
import pyautogui

def playMelon(hashtag):
    driver = webdriver.Chrome()
    url = 'https://melon.com'
    driver.get(url)
    driver.maximize_window()

    action = ActionChains(driver)

    driver.find_element_by_css_selector('.btn_login').click()
    driver.implicitly_wait(3)
    driver.find_element_by_xpath('//*[@id="conts_section"]/div/div/div[3]/button').click()

    #Enter ID
    driver.find_element_by_xpath('//*[@id="id"]').send_keys('hanil55')
    #Enter password
    driver.find_element_by_xpath('//*[@id="pwd"]').send_keys('7649612sy')

    driver.find_element_by_xpath('//*[@id="btnLogin"]').click()

    driver.find_element_by_xpath('//*[@id="gnb_menu"]/ul[1]/li[4]/a/span[2]').click()
    driver.find_element_by_xpath('//*[@id="djSearchKeyword"]').send_keys(hashtag)
    driver.find_element_by_xpath('//*[@id="conts"]/div[1]/div[2]/div[2]/button').click()
    driver.find_element_by_xpath('//*[@id="djPlylstList"]/div/ul/li[1]').click()

    driver.find_element_by_xpath('//*[@id="frm"]/div/table/thead/tr/th[1]/div/input').click()
    driver.find_element_by_xpath('//*[@id="frm"]/div/div/button[1]/span[2]').click()

    time.sleep(2)
    pyautogui.press('left')
    pyautogui.press('enter')
    time.sleep(100)
    return 0
