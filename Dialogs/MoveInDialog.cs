using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using EGDChatBot.Dialogs;
using EGDChatBot.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace EGDChatBot.Dialogs
{
    [Serializable]
    public class MoveInDialog : IDialog<bool>
    {

        private string m_Name;
        private string m_Email;
        private static readonly HttpClient m_Client = new HttpClient();
        private Customer m_Customer = new Customer();

        public async Task StartAsync(IDialogContext context)
        {
            if ((String.IsNullOrEmpty(m_Name)) && (String.IsNullOrEmpty(m_Email)))
            {
                //await context.PostAsync($"You selected Move-In option");
                await context.PostAsync($"Before we begin we need to verify your identity. We need some of your personal details in order to complete the verification process.");
                //await context.PostAsync($"We need some of your personal details in order to complete the verification process.");
            }

            if (String.IsNullOrEmpty(m_Name))
            {
                PromptDialog.Text(context, ResumeAfterNameEntered, "Please enter your first and last name");
            }

        }

        public async Task ResumeAfterNameEntered(IDialogContext context, IAwaitable<string> result)
        {
            m_Name = await result;

            //Parse out first and last name
            UnauthenticatedCustomer unauthenticaedCustomer = new UnauthenticatedCustomer();
            string[] fullName = m_Name.Split(' ');
            unauthenticaedCustomer.FirstName = fullName[0];
            unauthenticaedCustomer.LastName = fullName[1];

            //Call REST API to check if name exists. If yes, complete customer details are returned.
            string json = JsonConvert.SerializeObject(unauthenticaedCustomer);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            //var response = m_Client.PostAsync("http://localhost:7071/api/Authenticate", content).Result; // Sync
            var response = await m_Client.PostAsync("http://localhost:7071/api/Authenticate", content); //Async

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                m_Customer = JsonConvert.DeserializeObject<Customer>(jsonResponse);

                //TODO - Apply some regex to validate email address
                PromptDialog.Text(context, ResumeAfterEmailEntered, "Please enter your email address");
            }
            else
            {
                await context.PostAsync($"Sorry we could not find you in our files.");

                context.Done(false);

            }


        }

        public async Task ResumeAfterEmailEntered(IDialogContext context, IAwaitable<string> result)
        {
            m_Email = await result;

            //Verify email matches what is on file
            if(m_Email == m_Customer.Email)
            {
                await context.PostAsync($"Great!");

                PromptDialog.Confirm(context, ResumeAfterStreetCheck, $"Do you live at {m_Customer.StreetName}?");

            }
            else
            {
                await context.PostAsync($"Sorry your email doesn't match our files.");

                context.Done(false);
            }

        }

        public async Task ResumeAfterStreetCheck(IDialogContext context, IAwaitable<bool> result)
        {
            bool liveOnStreet = await result;

            PromptDialog.Number(context, ResumeAfterStreetNumberCheck, $"What street number do you live at?");

        }

        public async Task ResumeAfterStreetNumberCheck(IDialogContext context, IAwaitable<long> result)
        {
            long streetNumber = await result;

            if(m_Customer.HouseNumber == streetNumber)
            {
                await context.PostAsync($"Awesome! Your account has been verified. You are good to go.");

                await context.PostAsync($"You can use our web portal for requesting move in/out of our service area. Log in on http://www.enbridgegas.com/moving");

                context.Done(true);
            }
            else
            {
                await context.PostAsync($"Sorry your house number doesn't match our files.");

                context.Done(false);
            }


        }

        /*
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            await context.PostAsync($"Your name is {m_Name}");
            await context.PostAsync($"Your email is {m_Email}");

            context.Done(true);
        }
        */
    }
}