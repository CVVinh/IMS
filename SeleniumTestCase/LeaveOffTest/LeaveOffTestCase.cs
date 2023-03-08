using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using NUnit.Framework;

namespace SeleniumTestCase.LeaveOffTest
{
    public class LeaveOffTestCase
    {
        private IWebDriver driver;
        public IDictionary<string, object> vars { get; private set; }
        private IJavaScriptExecutor js;
        [SetUp]
        public void SetUp()
        {
            driver = new ChromeDriver();
            js = (IJavaScriptExecutor)driver;
            vars = new Dictionary<string, object>();
        }
        [TearDown]
        protected void TearDown()
        {
            driver.Quit();
        }
        [Test]
        public void GetAllLeaveOff_TestRecover_ReturnSuccessResult()
        {
            driver.Navigate().GoToUrl("http://localhost:5001/swagger/index.html");
            driver.Manage().Window.Size = new System.Drawing.Size(1141, 1023);
            driver.FindElement(By.CssSelector("#operations-LeaveOff-get_api_leaveOff_getAllLeaveOff .opblock-summary-control")).Click();
            driver.FindElement(By.CssSelector(".try-out__btn")).Click();
            driver.FindElement(By.CssSelector(".execute")).Click();
            driver.FindElement(By.CssSelector(".execute")).Click();
        }
    }
}
