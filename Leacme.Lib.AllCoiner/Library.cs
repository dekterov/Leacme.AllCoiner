// Copyright (c) 2017 Leacme (http://leac.me). View LICENSE.md for more information.
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CryptoCompare;

namespace Leacme.Lib.AllCoiner {

	public class Library {

		public Library() {
		}

		/// <summary>
		/// Get the conversion rate between a Cryptocoin and currency.
		/// /// </summary>
		/// <param name="client">The online Cryptocoin exchange monitor client.</param>
		/// <param name="amount">Amount to convert</param>
		/// <param name="coinSymbol">The unique exchange coin symbol.</param>
		/// <param name="currencySymbol">The unique currency three letter symbol.</param>
		/// <returns>The amount in currency.</returns>
		public async Task<decimal> GetPrice(CryptoCompareClient client, decimal amount, string coinSymbol, string currencySymbol) {
			return (await client.Prices.SingleSymbolPriceAsync(coinSymbol, new List<string> { currencySymbol })).First().Value * amount;
		}

		/// <summary>
		/// Get the country currencies for conversion.
		/// /// </summary>
		/// <returns></returns>
		public IList<RegionInfo> GetCurrencies() {
			var cl = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(
					z => { try { return new RegionInfo(z.LCID); } catch { return null; } }).Where(
						z => z != null).Where(
							z => !z.ISOCurrencySymbol.Equals("¤¤")).GroupBy(
								z => z.ISOCurrencySymbol).Select(
									z => z.First()).ToList();
			cl.Sort(Comparer<RegionInfo>.Create((z, zz) => z.ISOCurrencySymbol.CompareTo(zz.ISOCurrencySymbol)));
			return cl;
		}

		/// <summary>
		/// Get the Cryptocoin list.
		/// /// </summary>
		/// <param name="client">The online Cryptocoin exchange monitor client.</param>
		/// <returns>Cryptocoin symbol with its matching information.</returns>
		public async Task<IReadOnlyDictionary<string, CoinInfo>> GetAllCoins(CryptoCompareClient client) {
			return (await client.Coins.ListAsync()).Coins;
		}

		/// <summary>
		/// Get the online Cryptocoin exchange monitor client to make rate queries.
		/// /// </summary>
		/// <returns>The online CryptoCompareClient used for exchange monitoring.</returns>
		public CryptoCompareClient GetCoinService() {
			return new CryptoCompareClient();
		}

		/// <summary>
		/// Get top 20 Cryptocoin information by current pair-volume ratio.
		/// /// </summary>
		/// <param name="client">The online Cryptocoin exchange monitor client.</param>
		/// <returns>The top Cryptocoin information.</returns>
		public async Task<List<CoinSnapshotFullResponse>> GetTop20CoinDataInUSD(CryptoCompareClient client) {
			return (await Task.WhenAll((await client.Tops.ByPairVolumeAsync("USD")).Data.Where(zzz => !zzz.Id.Equals("-1")).Select(async z => await client.Coins.SnapshotFullAsync(int.Parse(z.Id))))).ToList();
		}

	}
}