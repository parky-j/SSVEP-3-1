using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using WMPLib;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;

namespace WindowsFormsApp7
{
    public partial class Form1 : Form
    {
        int counter = 0; int counter2 = 0; // 시간 재기위한 변수
        int userAge; // 나이대 저장용 변수
        int userSelection; // 첫 결정 (1 : 좌측, 2 : 중앙, 3 : 우측)
        int userSelection2; // 두 번째 결정 (1 : 좌측, 2 : 중앙, 3 : 우측)
        String[,] hashtags = new string[3, 3]; // 해시태그 저장 배열
        String[] firstHash = new string[3]; //임시 저장용 배열
        int firstSel; //(1 : 좌측, 2 : 중앙, 3 : 우측)
        int secondSel; //(1 : 좌측, 2 : 중앙, 3 : 우측)

        //파이썬 프로그램 실행을 위해 필요한 변수
        ProcessStartInfo psi;
        ProcessStartInfo psi2;

        String finalSelection;//유저의 최종 선택. "#힙합"과 같은 해시태그의 문자열로 전달됨.

        //파이썬 프로그램 내부에서 필요한 에러변수
        String error;
        //파이썬 프로그램 내부에서 필요한 변수. 결과적으로 예측된 주파수값의 문자열이 담기게 됨.
        String results;
        
        //멜론 사이트를 접속하기 위한 변수 설정
        protected ChromeDriverService _driverService = null;
        protected ChromeOptions _options = null;
        protected ChromeDriver _driver = null;

        //자극패널의 깜빡임을 동작하게 하는 메소드.
        private void blinkTextbox1(object sender, EventArgs e)
        {
            if (panel12.BackColor == Color.Green) panel12.BackColor = Color.White;
            else panel12.BackColor = Color.Green;
        }
        private void blinkTextbox2(object sender, EventArgs e)
        {
            if (panel13.BackColor == Color.Yellow) panel13.BackColor = Color.White;
            else panel13.BackColor = Color.Yellow;
        }
        private void blinkTextbox3(object sender, EventArgs e)
        {
            if (panel14.BackColor == Color.Blue) panel14.BackColor = Color.White;
            else panel14.BackColor = Color.Blue;
        }
        public Form1()
        {
            InitializeComponent();
            //평상시의 뇌파측정 자극을 주기전 실행
            //averagenormalbrainwave.py를 실행하기 위한 준비작업
            psi = new ProcessStartInfo();
            psi.FileName = @"C:\Users\user\AppData\Local\Programs\Python\Python38\python.exe"; //python 설치경로
            psi.Arguments = @"C:\Users\user\Desktop\3-1\신경공학\최종 시스템_201710773박영제\3by3\3by3\3by3_melon\3by3_melon\WindowsFormsApp7\averagenormalbrainwave.py";  //checkbrainwave.py 절대경로
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            //checkbrainwave.py를 실행하기 위한 준비작업
            psi2 = new ProcessStartInfo();
            psi2.FileName = psi.FileName;
            psi2.Arguments = @"C:\Users\user\Desktop\3-1\신경공학\최종 시스템_201710773박영제\3by3\3by3\3by3_melon\3by3_melon\WindowsFormsApp7\checkbrainwave.py"; //checkbrainwave.py 절대경로
            psi2.UseShellExecute = false;
            psi2.CreateNoWindow = true;
            psi2.RedirectStandardOutput = true;
            psi2.RedirectStandardError = true;

            //멜론사이트의 접속을 위해 _driverService와 _options 초기화
            _driverService = ChromeDriverService.CreateDefaultService();
            _driverService.HideCommandPromptWindow = true;
            _options = new ChromeOptions();
            _options.AddArgument("disable-gpu");
        }
        //checkbrainwave.py를 실행하는 메소드. 결과적으로 results에는 해당 자극패널에서 예측된 주파수의 문자열이 저장됨.
        public void responsebrain()
        {
            using (var process = Process.Start(psi2))
            {
                error = process.StandardError.ReadToEnd();
                results = process.StandardOutput.ReadToEnd();
            }
            Console.WriteLine(results);//결과값 문자열로 전달
        }
        //averagenormalbrainwave.py를 실행하는 메소드. 
        private void normalbrain()
        {
            using (var process = Process.Start(psi))
            {
                error = process.StandardError.ReadToEnd();
                results = process.StandardOutput.ReadToEnd();
            }//temporary.txt에 평균화한 데이터 생성
        }
        //첫 번째 자극패널에서의 timer.
        private void timer1_Tick(object sender, EventArgs e)
        {
            //counter는 시간을 재기 위해 필요하며, 4가 증가할 때마다 1초가 흐르도록 설정됨.
            counter++;
            //자극패널 제시 후 6초가 지났을 때
            if (counter == 24)
            {
                //responsebrain메소드를 실행해, 그 결과값에 따라 userSelection값 초기화
                responsebrain();
                int maxIdx = 0;
                if (results.Equals("35")) maxIdx = 1;
                if (results.Equals("13")) maxIdx = 2;
                if (results.Equals("23")) maxIdx = 3;

                userSelection = maxIdx;
            }
            //자극패널 제시 후 8초가 지났을 때
            if (counter == 32)
            {
                //결정된 userSelection의 값에 따라 다른 행동 취하게 함.
                if (userSelection == 1)
                {
                    //첫 번째 결정이 몇 번째 패널이었는지를 임시로 기억해둠
                    firstSel = 0;

                    //두 번째 선택에서 필요한 라벨 초기화
                    label10.Text = hashtags[0, 0];
                    label11.Text = hashtags[0, 1];
                    label20.Text = hashtags[0, 2];

                    //첫 번째 선택을 firstHash배열에 임시 저장
                    firstHash[0] = hashtags[0, 0];
                    firstHash[1] = hashtags[0, 1];
                    firstHash[2] = hashtags[0, 2];

                    //자극패널 보이지 않게 함.
                    panel12.Visible = false;
                    panel13.Visible = false;
                    panel14.Visible = false;

                    //선택된 패널의 색을 Pink로 하여, 사용자에게 다시 보여줌.
                    panel1.BackColor = Color.Transparent;
                    panel1.Visible = true;
                    panel2.Visible = true;
                    panel3.Visible = true;
                    panel4.Visible = true;
                    panel2.BackColor = Color.Pink;
                    label9.Visible = true;
                    label9.Text = "처리중입니다•••";
                    label9.AutoSize = false;
                    label9.TextAlign = ContentAlignment.MiddleCenter;
                }
                else if (userSelection == 2)
                {
                    //첫 번째 결정이 몇 번째 패널이었는지를 임시로 기억해둠
                    firstSel = 1;

                    //두 번째 선택에서 필요한 라벨 초기화
                    label10.Text = hashtags[1, 0];
                    label11.Text = hashtags[1, 1];
                    label20.Text = hashtags[1, 2];

                    //첫 번째 선택을 firstHash배열에 임시 저장
                    firstHash[0] = hashtags[1, 0];
                    firstHash[1] = hashtags[1, 1];
                    firstHash[2] = hashtags[1, 2];

                    //자극패널 보이지 않게 함.
                    panel12.Visible = false;
                    panel13.Visible = false;
                    panel14.Visible = false;

                    //선택된 패널의 색을 Pink로 하여, 사용자에게 다시 보여줌.
                    panel1.BackColor = Color.Transparent;
                    panel1.Visible = true;
                    panel2.Visible = true;
                    panel3.Visible = true;
                    panel4.Visible = true;
                    panel3.BackColor = Color.Pink;
                    label9.Visible = true;
                    label9.Text = "처리중입니다•••";
                    label9.AutoSize = false;
                    label9.TextAlign = ContentAlignment.MiddleCenter;
                }
                else if (userSelection == 3)
                {
                    //첫 번째 결정이 몇 번째 패널이었는지를 임시로 기억해둠
                    firstSel = 2;

                    //두 번째 선택에서 필요한 라벨 초기화
                    label10.Text = hashtags[2, 0];
                    label11.Text = hashtags[2, 1];
                    label20.Text = hashtags[2, 2];

                    //첫 번째 선택을 firstHash배열에 임시 저장
                    firstHash[0] = hashtags[2, 0];
                    firstHash[1] = hashtags[2, 1];
                    firstHash[2] = hashtags[2, 2];

                    //자극패널 보이지 않게 함.
                    panel12.Visible = false;
                    panel13.Visible = false;
                    panel14.Visible = false;

                    //선택된 패널의 색을 Pink로 하여, 사용자에게 다시 보여줌.
                    panel1.BackColor = Color.Transparent;
                    panel1.Visible = true;
                    panel2.Visible = true;
                    panel3.Visible = true;
                    panel4.Visible = true;
                    panel4.BackColor = Color.Pink;
                    label9.Visible = true;
                    label9.Text = "처리중입니다•••";
                    label9.AutoSize = false;
                    label9.TextAlign = ContentAlignment.MiddleCenter;
                }
            }
            //자극패널 제시 후 9초가 지났을 때
            if (counter == 36)
            {
                //두 번째 선택을 위한 패널들을 보여주고, 사용자에게 띄워줄 문구를 수정함.
                //또한, 필요하지 않은 패널들을 보이지 않게 함. timer의 동작을 멈춤.
                panel6.BackColor = Color.Yellow;
                panel7.BackColor = Color.Yellow;
                panel8.BackColor = Color.Yellow;
                panel11.Visible = true;
                panel6.Visible = true;
                panel7.Visible = true;
                panel8.Visible = true;
                pictureBox1.Visible = false;
                panel2.Visible = false;
                panel3.Visible = false;
                panel4.Visible = false;
                label9.Text = "선택한 해시태그 중 원하는 해시태그의 위치를\n 파악 후, 아래의 '시작'버튼을 누르세요.";
                button6.Visible = true;
                timer1.Stop();
                counter = 0;
                button1.Visible = false;
            }
        }

        //첫 번째 선택을 위한 자극패널에 진입하는 버튼
        private void button1_Click(object sender, EventArgs e)
        {
            //자극패널에서 보여서는 안될 패널을 보이지 않게 함.
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;

            //자극패널에 해당하는 화면을 사용자에게 보이게 함.
            pictureBox1.Visible = true;
            panel12.Visible = true;
            panel13.Visible = true;
            panel14.Visible = true;

            //timer1의 시작
            timer1.Interval = 500;
            timer1.Tick += timer1_Tick;
            timer1.Start();

            //자극패널에서 보여서는 안될 버튼/라벨을 보이지 않게 함.
            button1.Visible = false;
            button2.Visible = false;
            button6.Visible = false;
            button1.Enabled = false;
            label9.Visible = false;
        }

        //두 번째 선택을 위한 자극패널에 진입하는 버튼
        private void button6_Click(object sender, EventArgs e)
        {
            //자극패널에서 보여서는 안될 패널을 보이지 않게 함.
            panel6.Visible = false;
            panel7.Visible = false;
            panel8.Visible = false;
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;

            //자극패널에 해당하는 화면을 사용자에게 보이게 함.
            pictureBox1.Visible = true;
            panel12.Visible = true;
            panel13.Visible = true;
            panel14.Visible = true;

            //timer2의 시작
            timer2.Interval = 500;
            timer2.Tick += timer2_Tick;
            timer2.Start();

            //자극패널에서 보여서는 안될 버튼/라벨/패널을 보이지 않게 함.
            panel11.Visible = false;
            button6.Visible = false;
            button6.Enabled = false;
            label9.Visible = false;
        }

        //두 번째 선택에서의 timer.
        private void timer2_Tick(object sender, EventArgs e)
        {
            //마찬가지로 counter는 4만큼 커질 때마다 1초가 흐르도록 설정됨.
            counter2++;
            //자극패널 제시 후 6초가 지났을 때
            if (counter2 == 24)
            {
                //responsebrain메소드를 실행해, 그 결과값에 따라 userSelection2값 초기화
                responsebrain();

                int maxIdx = 0;
                if (results.Equals("35")) maxIdx = 1;
                if (results.Equals("13")) maxIdx = 2;
                if (results.Equals("23")) maxIdx = 3;

                userSelection2 = maxIdx;
            }
            //자극패널 제시 후 8초가 지났을 때
            if (counter2 == 32)
            {
                //결정된 userSelection의 값에 따라 다른 행동 취하게 함.
                if (userSelection2 == 1)
                {
                    //첫 번째 결정이 몇 번째 패널이었는지를 임시로 기억해둠
                    secondSel = 0;

                    //선택된 패널의 색을 Pink로 하여, 사용자에게 다시 보여줌.
                    panel11.Visible = true;
                    panel11.BackColor = Color.Transparent;
                    panel6.Visible = true;
                    panel7.Visible = true;
                    panel8.Visible = true;
                    panel6.BackColor = Color.Pink;

                    //자극패널을 유저에게 보이지 않게 함
                    panel12.Visible = false;
                    panel13.Visible = false;
                    panel14.Visible = false;

                    //최종선택된 해시태그를 finalSelection에 저장
                    finalSelection = firstHash[0];
                    //유저에게 보여주기 위해 라벨의 Text 수정
                    label23.Text = finalSelection;

                    //다음 화면을 유저에게 보여주기 위해 라벨/버튼 등을 정리
                    pictureBox1.Visible = false;
                    label9.Visible = true;
                    label9.Text = "결과 확인을 위해 '다음' 버튼을 누르세요.";
                    label9.AutoSize = false;
                    label9.TextAlign = ContentAlignment.MiddleCenter;
                    button6.Visible = false;
                    button2.Visible = true;
                }
                else if (userSelection2 == 2)
                {
                    //첫 번째 결정이 몇 번째 패널이었는지를 임시로 기억해둠
                    secondSel = 1;

                    //선택된 패널의 색을 Pink로 하여, 사용자에게 다시 보여줌.
                    panel11.Visible = true;
                    panel11.BackColor = Color.Transparent;
                    panel6.Visible = true;
                    panel7.Visible = true;
                    panel8.Visible = true;
                    panel7.BackColor = Color.Pink;

                    //자극패널을 유저에게 보이지 않게 함
                    panel12.Visible = false;
                    panel13.Visible = false;
                    panel14.Visible = false;

                    //최종선택된 해시태그를 finalSelection에 저장
                    finalSelection = firstHash[1];
                    //유저에게 보여주기 위해 라벨의 Text 수정
                    label23.Text = finalSelection;

                    //다음 화면을 유저에게 보여주기 위해 라벨/버튼 등을 정리
                    pictureBox1.Visible = false;
                    label9.Visible = true;
                    label9.Text = "결과 확인을 위해 '다음' 버튼을 누르세요.";
                    label9.AutoSize = false;
                    label9.TextAlign = ContentAlignment.MiddleCenter;
                    button6.Visible = false;
                    button2.Visible = true;
                }
                else if (userSelection2 == 3)
                {
                    //첫 번째 결정이 몇 번째 패널이었는지를 임시로 기억해둠
                    secondSel = 2;

                    //선택된 패널의 색을 Pink로 하여, 사용자에게 다시 보여줌.
                    panel11.Visible = true;
                    panel11.BackColor = Color.Transparent;
                    panel6.Visible = true;
                    panel7.Visible = true;
                    panel8.Visible = true;
                    panel8.BackColor = Color.Pink;

                    //자극패널을 유저에게 보이지 않게 함
                    panel12.Visible = false;
                    panel13.Visible = false;
                    panel14.Visible = false;

                    //최종선택된 해시태그를 finalSelection에 저장
                    finalSelection = firstHash[2];
                    //유저에게 보여주기 위해 라벨의 Text 수정
                    label23.Text = finalSelection;

                    //다음 화면을 유저에게 보여주기 위해 라벨/버튼 등을 정리
                    pictureBox1.Visible = false;
                    label9.Visible = true;
                    label9.Text = "결과 확인을 위해 '다음' 버튼을 누르세요.";
                    label9.AutoSize = false;
                    label9.TextAlign = ContentAlignment.MiddleCenter;
                    button6.Visible = false;
                    button2.Visible = true;
                }
            }
        }

        //최종선택 이후 마지막 화면으로 넘어가기 위한 버튼
        private void button2_Click(object sender, EventArgs e)
        {
            //최종선택된 해시태그에 맞는 플레이리스트 사진 유저에게 보여줌
            pictureBox2.Image = Image.FromFile("hashtags/" + userAge + "/" + firstSel + " " + secondSel + ".png");

            //마지막 화면을 유저에게 보여주기 위해 필요한 패널을 보이게, 필요하지 않은 패널/버튼을 보이지 않게 함
            panel6.Visible = false;
            panel7.Visible = false;
            panel8.Visible = false;
            panel11.Visible = false;
            panel10.Visible = true;
            button2.Visible = false;
        }

        //마지막 화면의 플레이리스트 재생을 위한 버튼
        private void button3_Click(object sender, EventArgs e)
        {
            //본인의 id와 password를 초기화해둠.
            //개인정보이기에 설정하지 않았으나, 시연에서는 id와 password를 미리 설정하여 시연할 예정.
            string id = "";
            string password = "";

            //_driver초기화 후 melon.com에 접속
            _driver = new ChromeDriver(_driverService, _options);
            _driver.Navigate().GoToUrl("https://melon.com");
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            //로그인 화면까지 이동
            var element = _driver.FindElementByClassName("btn_login");
            element.Click();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            element = _driver.FindElementByXPath("//*[@id='conts_section']/div/div/div[3]/button");
            element.Click();
            
            //id입력
            element = _driver.FindElementByXPath("//*[@id='id']");
            element.SendKeys(id);

            //password 입력
            element = _driver.FindElementByXPath("//*[@id='pwd']");
            element.SendKeys(password);

            //로그인 버튼 클릭
            element = _driver.FindElementByXPath("//*[@id='btnLogin']");
            element.Click();

            //finalSelection입력을 위한 페이지로 이동
            element = _driver.FindElementByXPath("//*[@id='gnb_menu']/ul[1]/li[4]/a/span[2]");
            element.Click();

            //finalSelection입력
            element = _driver.FindElementByXPath("//*[@id='djSearchKeyword']");
            element.SendKeys(finalSelection);

            //검색버튼 클릭
            element = _driver.FindElementByXPath("//*[@id='conts']/div[1]/div[2]/div[2]/button");
            element.Click();

            //선택된 플레이리스트를 재생
            element = _driver.FindElementByXPath("//*[@id='djPlylstList']/div/ul/li[1]/div[1]/a/span");
            element.Click();
            element = _driver.FindElementByXPath("//*[@id='frm']/div/table/thead/tr/th[1]/div/input");
            element.Click();
            element = _driver.FindElementByXPath("//*[@id='frm']/div/div/button[1]/span[2]");
            element.Click();

            //크롬 브라우저 자체적으로 발생하는 경고문 처리 위한 코드
            System.Threading.Thread.Sleep(2000);
            SendKeys.Send("{LEFT}");
            SendKeys.Send("{ENTER}");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //normalbrain함수를 실행해 평상시의 뇌파데이터를 확인
            normalbrain();

            //유저가 선택한 연령대에 따라 보여줄 해시태그 달리 초기화
            if (userAge == 10)
            {
                hashtags[0, 0] = "#하이틴";
                hashtags[0, 1] = "#힙합";
                hashtags[0, 2] = "#아이돌";

                hashtags[1, 0] = "#틱톡";
                hashtags[1, 1] = "#힘들때";
                hashtags[1, 2] = "#기분전환";

                hashtags[2, 0] = "#여행";
                hashtags[2, 1] = "#팝";
                hashtags[2, 2] = "#인디";
            }
            else if (userAge == 20)
            {
                hashtags[0, 0] = "#추억";
                hashtags[0, 1] = "#힐링";
                hashtags[0, 2] = "#감성";

                hashtags[1, 0] = "#청춘";
                hashtags[1, 1] = "#드라이브";
                hashtags[1, 2] = "#명곡";

                hashtags[2, 0] = "#스트레스";
                hashtags[2, 1] = "#사랑";
                hashtags[2, 2] = "#클럽";
            }
            else if (userAge == 30)
            {
                hashtags[0, 0] = "#추억";
                hashtags[0, 1] = "#발라드";
                hashtags[0, 2] = "#드라이브";

                hashtags[1, 0] = "#감성";
                hashtags[1, 1] = "#휴식";
                hashtags[1, 2] = "#출근길";

                hashtags[2, 0] = "#슬픔";
                hashtags[2, 1] = "#퇴근길";
                hashtags[2, 2] = "#세대공감";
            }
            else if (userAge == 40)
            {
                hashtags[0, 0] = "#트롯";
                hashtags[0, 1] = "#그때그노래";
                hashtags[0, 2] = "#익숙한";

                hashtags[1, 0] = "#유명한";
                hashtags[1, 1] = "#추억";
                hashtags[1, 2] = "#잔잔한";

                hashtags[2, 0] = "#힐링";
                hashtags[2, 1] = "#추천곡";
                hashtags[2, 2] = "#ost";
            }

            //초기화된 해시태그를 바탕으로 첫 결정을 위한 화면에서 유저에게 보여줄 해시태그 설정
            label1.Text = hashtags[0, 0];
            label2.Text = hashtags[0, 1];
            label3.Text = hashtags[0, 2];

            label4.Text = hashtags[1, 0];
            label5.Text = hashtags[1, 1];
            label6.Text = hashtags[1, 2];

            label7.Text = hashtags[2, 0];
            label8.Text = hashtags[2, 1];
            label12.Text = hashtags[2, 2];

            //해시태그 선택화면을 온전히 보여주기 위해 필요한 패널은 보이게, 필요하지 않은 패널은 보이지 않게 함
            pictureBox1.Visible = false;
            panel2.BackColor = Color.Yellow;
            panel3.BackColor = Color.Yellow;
            panel4.BackColor = Color.Yellow;
            panel5.BackColor = Color.Transparent;
            panel6.Visible = false;
            panel7.Visible = false;
            panel8.Visible = false;
            panel9.Visible = false;
            panel10.Visible = false;
            panel11.Visible = false;
            panel12.Visible = false;
            panel13.Visible = false;
            panel14.Visible = false;

            //불필요한 버튼 보이지 않게 함
            button2.Visible = false;
            button6.Visible = false;

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////자극패널 주파수를 정하는 구간입니다////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////

            // 좌측 자극(35Hz)
            timer5.Tick += blinkTextbox1;
            timer5.Interval = 1000 / 35 / 2;
            timer5.Enabled = true;

            // 중앙 자극(13Hz)
            timer6.Tick += blinkTextbox2;
            timer6.Interval = 1000 / 13 / 2;
            timer6.Enabled = true;

            // 우측 자극(23Hz)
            timer7.Tick += blinkTextbox3;
            timer7.Interval = 1000 / 23 / 2;
            timer7.Enabled = true;

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////자극패널 주파수를 정하는 구간입니다////////////////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////////////////

        }

        //연령대 선택 화면에서의 버튼
        private void button5_Click(object sender, EventArgs e)
        {
            //선택한 연령대에 따라 userAge변수 초기화
            if (radioButton1.Checked == true)
            {
                userAge = 10;
            }

            else if (radioButton2.Checked == true)
            {
                userAge = 20;
            }

            else if (radioButton3.Checked == true)
            {
                userAge = 30;
            }

            else
            {
                userAge = 40;
            }

            //연령대 선택 화면을 보이지 않게 함
            panel15.Visible = false;
        }

        //첫 화면에서의 버튼
        private void button7_Click(object sender, EventArgs e)
        {
            //첫 화면을 보이지 않게 함.
            panel16.Visible = false; 
        }
    }
}
