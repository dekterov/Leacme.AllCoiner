// Copyright (c) 2017 Leacme (http://leac.me). View LICENSE.md for more information.
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Leacme.Lib.AllCoiner;

namespace Leacme.App.AllCoiner {

	public class AppUI {

		private StackPanel rootPan = (StackPanel)Application.Current.MainWindow.Content;
		private Library lib = new Library();

		public AppUI() {

			var blurb1 = App.TextBlock;
			blurb1.TextAlignment = TextAlignment.Center;
			blurb1.Text = "Get the current rate for any cryptocoin in your currency of choice.";

			var amtPanel = App.HorizontalFieldWithButton;
			amtPanel.holder.HorizontalAlignment = HorizontalAlignment.Center;
			amtPanel.label.Text = "Amount:";
			amtPanel.field.Text = "1";
			amtPanel.button.IsEnabled = false;
			amtPanel.button.Content = "Convert";

			var menuHdr = App.HorizontalStackPanel;
			menuHdr.HorizontalAlignment = HorizontalAlignment.Center;

			var coinDrpn = App.AutoCompleteWithLabel;
			coinDrpn.label.Text = "Cryptocoin:";

			var toHldr = App.TextBlock;
			toHldr.Text = "To";

			var curcyDrpn = App.ComboBoxWithLabel;
			curcyDrpn.label.Text = "Currency:";

			curcyDrpn.comboBox.Items = lib.GetCurrencies().Select(z => z.ISOCurrencySymbol + " - " + z.CurrencyEnglishName).ToList();
			curcyDrpn.comboBox.SelectedItem = ((List<string>)curcyDrpn.comboBox.Items).First(z => ((string)z).StartsWith("USD "));

			menuHdr.Children.AddRange(new List<IControl>() { coinDrpn.holder, toHldr, curcyDrpn.holder });

			var rtHode = App.TextBlock;
			rtHode.FontWeight = FontWeight.Bold;
			rtHode.FontSize = 30;
			rtHode.TextAlignment = TextAlignment.Center;
			rtHode.Text = "";
			var rtHode2 = App.TextBlock;
			rtHode2.TextAlignment = TextAlignment.Center;
			rtHode2.Text = "";

			Dispatcher.UIThread.InvokeAsync(async () => {
				var coins = await lib.GetAllCoins(lib.GetCoinService());
				coinDrpn.acBox.Items = coins.Select(z => z.Value.Symbol + " - " + z.Value.CoinName).OrderBy(z => z).ToList();
				coinDrpn.acBox.SelectedItem = ((List<string>)coinDrpn.acBox.Items).First(z => ((string)z).StartsWith("BTC "));
				amtPanel.button.IsEnabled = true;
				amtPanel.button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
			});

			amtPanel.button.Click += async (z, zz) => {

				if (decimal.TryParse(amtPanel.field.Text, out var amt)) {
					try {
						var enteredSubstringCoinSymbol = ((string)coinDrpn.acBox.SelectedItem).Substring(0, ((string)coinDrpn.acBox.SelectedItem).IndexOf(" "));
						var enteredSubstringCurrencySymbol = ((string)curcyDrpn.comboBox.SelectedItem).Substring(0, ((string)curcyDrpn.comboBox.SelectedItem).IndexOf(" "));
						var resp = await lib.GetPrice(lib.GetCoinService(), amt, enteredSubstringCoinSymbol, enteredSubstringCurrencySymbol);

						rtHode.Text = amt + " " + enteredSubstringCoinSymbol + " = " + resp + " " + enteredSubstringCurrencySymbol;
						rtHode2.Text = "(1 " + enteredSubstringCoinSymbol + " = " + resp / amt + " " + enteredSubstringCurrencySymbol + ", "
							+ " 1 " + enteredSubstringCurrencySymbol + " = " + amt / resp + " " + enteredSubstringCoinSymbol + ")";

					} catch (Exception) {
						rtHode.Text = "Unable to convert. Check your units and the amount.";
					}
				}

			};

			var tBrb = App.TextBlock;
			tBrb.TextAlignment = TextAlignment.Center;
			tBrb.Text = "\nCurrent Top 20 Cryptocoin Information";

			var tBtbPnl = App.DataGrid;
			tBtbPnl.Height = 275;
			Dispatcher.UIThread.InvokeAsync(async () => {
				tBtbPnl.Items = (await lib.GetTop20CoinDataInUSD(lib.GetCoinService())).Select(
					z => new { z.Data.General.Symbol, z.Data.General.Name, z.Data.General.Id, z.Data.General.Algorithm, z.Data.General.NetHashesPerSecond, z.Data.General.BlockNumber, z.Data.General.BlockTime, z.Data.General.BlockReward, z.Data.General.BlockRewardReduction, z.Data.General.StartDate, z.Data.General.TotalCoinsMined, z.Data.General.TotalCoinSupply, z.Data.General.LastBlockExplorerUpdateTS }
				);
			});

			rootPan.Children.AddRange(new List<IControl>() { blurb1, amtPanel.holder, menuHdr, rtHode, rtHode2, tBrb, tBtbPnl });

		}
	}
}