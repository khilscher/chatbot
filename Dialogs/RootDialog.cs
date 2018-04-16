using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace EGDChatBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string MoveInOption = "Move-In";
        private const string BalanceInquiryOption = "Balance Inquiry";
        private const string ReportingPaymentOption = "Reporting Payment";
        private const string PaymentArrangementOption = "Payment Arrangement";
        private const string DemoOption = "Cards Dialog Demo";

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {

            await context.PostAsync("Welcome to the chat desk. We are here to help you.");

            PromptDialog.Choice(
                context,
                this.AfterChoiceSelected,
                new[] { MoveInOption, BalanceInquiryOption, ReportingPaymentOption, PaymentArrangementOption, DemoOption },
                "What can I help you with?",
                "I am sorry but I didn't understand that. I need you to select one of the options below.",
                attempts: 2);
        }

        private async Task AfterChoiceSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var selection = await result;

                switch (selection)
                {
                    case MoveInOption:
                        context.Call(new MoveInDialog(), this.AfterMoveIn);
                        break;

                    case DemoOption:
                        context.Call(new CardsDialog(), this.AfterDemo);
                        break;

                    case BalanceInquiryOption:
                        await context.PostAsync("This functionality is not yet implemented!");
                        await this.StartAsync(context);
                        break;

                    case ReportingPaymentOption:
                        await context.PostAsync("This functionality is not yet implemented!");
                        await this.StartAsync(context);
                        break;

                    case PaymentArrangementOption:
                        await context.PostAsync("This functionality is not yet implemented!");
                        await this.StartAsync(context);
                        break;

                        


                }
            }
            catch (TooManyAttemptsException)
            {
                await this.StartAsync(context);
            }
        }

        private async Task AfterMoveIn(IDialogContext context, IAwaitable<bool> result)
        {
            var success = await result;

            if (!success)
            {
                await context.PostAsync("Your Move-In was not successful");
            }

            await this.StartAsync(context);

        }

        private async Task AfterDemo(IDialogContext context, IAwaitable<object> result)
        {

            await this.StartAsync(context);

        }
    }
}