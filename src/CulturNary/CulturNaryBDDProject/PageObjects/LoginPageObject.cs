using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI; // Add this line
using SeleniumExtras.WaitHelpers; // Add this line
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturNaryBDDProject.PageObjects
{
    public class LoginPageObject : PageObject
    {
        public LoginPageObject(IWebDriver webDriver) : base(webDriver)
        {
            _pageName = "Login";

            if(_webDriver == null)
            {
                throw new ArgumentNullException("webDriver");
            }
        }

        public IWebElement UsernameInput => _webDriver.FindElement(By.Id("Input_Email"));
        public IWebElement PasswordInput => _webDriver.FindElement(By.Id("Input_Password"));
        public IWebElement RememberMeCheck => _webDriver.FindElement(By.Id("Input_RememberMe"));
        public IWebElement SubmitButton => _webDriver.FindElement(By.Id("login-submit"));

        public void EnterUsername(string username)
        {
            WebDriverWait wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(10));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("Input_Email")));

            UsernameInput.Clear();
            UsernameInput.SendKeys(username);
        }

        public void EnterPassword(string password)
        {
            WebDriverWait wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(10));
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("Input_Password")));
            
            PasswordInput.Clear();
            PasswordInput.SendKeys(password);
        }

        public void SetRememberMe(bool rememberMe)
        {
            if(rememberMe)
            {
                if(!RememberMeCheck.Selected)
                {
                    RememberMeCheck.Click();
                }
            }
            else
            {
                if(RememberMeCheck.Selected)
                {
                    RememberMeCheck.Click();
                }
            }
        }

        public void Login()
        {
            SubmitButton.Click();
        }

        public bool HasLoginErrors()
        {
           ReadOnlyCollection<IWebElement> elements = _webDriver.FindElements(By.CssSelector(".validation-summary-errors"));
           return elements.Count() > 0;
        }
    }
}