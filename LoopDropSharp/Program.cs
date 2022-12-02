using CsvHelper;
using LoopDropSharp;
using LoopDropSharp.Helpers;
using LoopDropSharp.Models;
using LoopringSharp;
using Microsoft.Extensions.Configuration;
using Nethereum.ABI.Util;
using Nethereum.Signer;
using Nethereum.Signer.EIP712;
using Nethereum.Util;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Tls;
using PoseidonSharp;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using Type = LoopDropSharp.Type;

// load services
ILoopringService loopringService = new LoopringService();
ILoopExchangeService loopExchangeService = new LoopExchangeService();
IImxService imxService = new ImxService();
IEthereumService ethereumService = new EthereumService();
INftMetadataService nftMetadataService = new NftMetadataService("https://loopring.mypinata.cloud/ipfs/");

//load global variables 
List<MintsAndTotal> userMintsAndTotalList = new List<MintsAndTotal>();
List<NftHolder> nftHoldersAndTotalList = new List<NftHolder>();
List<NftHolder> nftHoldersList = new List<NftHolder>();
List<string> invalidAddress = new List<string>();
List<string> validAddress = new List<string>();
List<string> banishAddress = new List<string>();
NftBalance userNftToken = new NftBalance();
NftMetadata nftMetadata = new NftMetadata();
MinterAndCollection minterAndCollection = new MinterAndCollection();
string nftData;
string nftAmount;
int nftTokenId;
string userResponseOnWalletAddressDisplay;
string nftMetadataLink;
string toAddressInitial;
string toAddressInitialAndAmount;
bool contains = false;
var fileName = "Input.txt";
int airdropNumberOn;

//Utils.CheckAppsettingsDotJson();

//Settings loaded from the appsettings.json fileq
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("./appsettings.json")
    .AddEnvironmentVariables()
    .Build();
Settings settings = config.GetRequiredSection("Settings").Get<Settings>();

string loopringApiKey = settings.LoopringApiKey;//loopring api key KEEP PRIVATE
string loopringPrivateKey = settings.LoopringPrivateKey; //loopring private key KEEP PRIVATE
var MMorGMEPrivateKey = settings.MMorGMEPrivateKey; //metamask or gamestop private key KEEP PRIVATE
var fromAddress = settings.LoopringAddress; //your loopring address
var fromAccountId = settings.LoopringAccountId; //your loopring account id
var validUntil = settings.ValidUntil; //the examples seem to use this number
var maxFeeTokenId = settings.MaxFeeTokenId; //0 should be for ETH, 1 is for LRC
var exchange = settings.Exchange; //loopring exchange address, shouldn't need to change this,
int toAccountId = 0; //leave this as 0 DO NOT CHANGE

var userResponseReadyToMoveOn = Menu.BannerForLoopDropSharp();

var signedMessage = EDDSAHelper.EddsaSignUrl(
loopringPrivateKey,
HttpMethod.Get,
new List<(string Key, string Value)>() { ("accountId", fromAccountId.ToString()) },
null,
Constants.ApiKeyUrl,
"https://api3.loopring.io/");
//"https://uat2.loopring.io/");
var apiKey = await loopringService.GetApiKey(fromAccountId, signedMessage);

if (apiKey != loopringApiKey)
{
    Font.SetTextToYellow("The appsettings.json does not appear to be setup correctly.");
    Font.SetTextToYellow($"Please check the values in the appsetting.json and restart the application.");

}

while (userResponseReadyToMoveOn == "yes")
{
    var menuAndUtility = Menu.MenuForLoopDropSharp();

    switch (menuAndUtility.userResponseOnUtility)
    {
        #region case 0
        case "0":
            Font.SetTextToBlue(menuAndUtility.allUtilities.ElementAt(0).Value);
            Font.SetTextToPurple("     What can this application do?");
            Font.SetTextToWhite("\t LoopDropSharp can airdrop Nfts and mass transfer Crypto");
            Font.SetTextToWhite("\t You can find who holds a specifc Nft and easily reward");
            Font.SetTextToWhite("\t those that do.");
            Console.WriteLine();
            Font.SetTextToPurple("     What is Nft Data used for?");
            Font.SetTextToWhite("\t Nft Data is used to find your unique Nft on the blockchain.");
            Font.SetTextToWhite("\t Once you have the Nft Data you are able to find Nft Holders ");
            Font.SetTextToWhite("\t and transfer Nfts.");
            Console.WriteLine();
            Font.SetTextToPurple("     Is Nft Data and same as Nft Id?");
            Font.SetTextToWhite("\t Nft Data is not the same as an Nft Id, but Nft Data comes");
            Font.SetTextToWhite("\t from an Nft Id.");
            Font.SetTextToWhite("\t You can find the Nft Id on any Loopring Explorer and");
            Font.SetTextToWhite("\t typically any marketplace.");
            Console.WriteLine();
            Font.SetTextToPurple("     What wallet can I use to Airdrop and transfer?");
            Font.SetTextToWhite("\t You can use a MetaMask wallet or a GameStop Wallet.");
            Font.SetTextToWhite("\t For those wondering, unfortunately, you cannot use a");
            Font.SetTextToWhite("\t Loopring wallet to perform Airdrops.");
            Console.WriteLine();
            break;
        #endregion
        #region case 1
        case "1":
            NftData nftdataRequest;
            Font.SetTextToBlue(menuAndUtility.allUtilities.ElementAt(1).Value);
            Font.SetTextToBlue("Here you will get Nft Data from an Nft.");
            Font.SetTextToDarkGray("Find your Nft on lexplorer.io or explorer.loopring.io and find the below.");
            Font.SetTextToWhite("Let's get started.");
            do
            {
                Font.SetTextToBlue("Enter in the Nft Id");
                string nftIdForNftData = Utils.ReadLineWarningNoNulls("Enter in the Nft Id");
                minterAndCollection = UtilsLoopring.GetMinterAndCollection();

                minterAndCollection.minter = await loopringService.CheckForEthAddress(settings.LoopringApiKey, minterAndCollection.minter);

                nftdataRequest = await loopringService.GetNftData(settings.LoopringApiKey, nftIdForNftData, minterAndCollection.minter, minterAndCollection.TokenId);  //the nft tokenId, not the nftId
            } while (nftdataRequest == null);

            Font.SetTextToBlueInline($"This Nft's Nft Data is: ");
            Font.SetTextToGreen($"{nftdataRequest.nftData}");
            break;
        #endregion case 1
        #region case 2
        case "2":
            Font.SetTextToBlue(menuAndUtility.allUtilities.ElementAt(2).Value);
            Font.SetTextToBlue("Here you will provide the Nft Id and get the associated Nft Data.");
            Font.SetTextToYellow("All Nft Ids have to be from the same collection.");
            Font.SetTextToWhite("Let's get started.");
            var howManyWallets = Utils.CheckInputDotTxt(fileName);
            string userResponseOnNftData;

            using (StreamReader sr = new StreamReader($"./{fileName}"))
            {
                string nftIdFromText;
                nftdataRequest = null;
                minterAndCollection = UtilsLoopring.GetMinterAndCollection();
                var counter = 0;
                while ((nftIdFromText = sr.ReadLine()) != null)
                {
                    do
                    {
                        var nftIdForNftData = nftIdFromText;
                        if (minterAndCollection.minter.Contains(".eth"))
                        {
                            var varHexAddress = await loopringService.GetHexAddress(settings.LoopringApiKey, minterAndCollection.minter);
                            if (!String.IsNullOrEmpty(varHexAddress.data))
                            {
                                minterAndCollection.minter = varHexAddress.data;
                            }
                            else
                            {
                                Console.WriteLine($"{minterAndCollection.minter} is an invalid address");
                                continue;
                            }
                        }

                        nftdataRequest = await loopringService.GetNftData(settings.LoopringApiKey, nftIdForNftData, minterAndCollection.minter, minterAndCollection.TokenId);
                    } while (nftdataRequest == null);
                    if (counter == 0)
                    {
                        Font.SetTextToBlue("Nft Data:");
                    }
                    counter++;
                    Font.SetTextToGreen(nftdataRequest.nftData);
                }
            }
            break;
        #endregion case 2
        #region case 3
        case "3":
            userResponseOnWalletAddressDisplay = "";
            string userResponseOnMinter;
            string responseOnMinter;
            AccountInformation responseOnAccountId = new AccountInformation();
            Font.SetTextToBlue(menuAndUtility.allUtilities.ElementAt(3).Value);
            Font.SetTextToBlue("Here you will get all the Nft Data from the Nfts in a Collection.");
            Font.SetTextToWhite("Let's get started.");
            do
            {
                minterAndCollection = UtilsLoopring.GetMinterAndCollection();
                responseOnMinter = await loopringService.CheckForEthAddress(settings.LoopringApiKey, minterAndCollection.minter);
                responseOnAccountId = await loopringService.GetUserAccountInformationFromOwner(responseOnMinter);
            }
            while (responseOnAccountId == null);
            var nftDataList = await loopringService.GetUserMintedNftsWithCollection(settings.LoopringApiKey, responseOnAccountId.accountId, minterAndCollection.TokenId);
            Font.SetTextToGreen($"{minterAndCollection.minter} has {nftDataList.Count()} mints in this Collection.");

            foreach (var nftDataSingle in nftDataList)
            {
                userNftToken = await loopringService.GetTokenId(settings.LoopringApiKey, responseOnAccountId.accountId, nftDataSingle.nftData);
                nftTokenId = userNftToken.data[0].tokenId;
                nftMetadataLink = await ethereumService.GetMetadataLink(userNftToken.data[0].nftId, userNftToken.data[0].tokenAddress, 0);
                nftMetadata = await nftMetadataService.GetMetadata(nftMetadataLink);
                if (nftMetadata == null)
                {

                    Console.WriteLine($"NONEFOUND {userNftToken.data[0].nftId},{responseOnAccountId.owner} {userNftToken.data[0].tokenAddress}");
                }
                else
                {
                    Font.SetTextToGreen($"{nftMetadata.name}, {nftDataSingle.nftData}");
                }
            }
            break;
        #endregion case 3
        #region case 4
        case "4":
            responseOnAccountId = null;
            string ensOrWalletAddress;
            Font.SetTextToBlue(menuAndUtility.allUtilities.ElementAt(4).Value);
            Font.SetTextToBlue("Here you will get all the Nft Data from the Nfts in a wallet.");
            Font.SetTextToWhite("Let's get started.");
            do
            {
                Font.SetTextToBlue("Enter in the ens or wallet address");
                ensOrWalletAddress = Utils.ReadLineWarningNoNulls("Enter in the ens or wallet address");
                responseOnMinter = await loopringService.CheckForEthAddress(settings.LoopringApiKey, ensOrWalletAddress);
                responseOnAccountId = await loopringService.GetUserAccountInformationFromOwner(responseOnMinter);
            }
            while (responseOnAccountId == null);
            var walletsNftBalance = await loopringService.GetWalletsNfts(settings.LoopringApiKey, responseOnAccountId.accountId);
            Font.SetTextToGreen($"{ensOrWalletAddress} has {walletsNftBalance.Count()} Nfts in their wallet.");

            foreach (var singleNftBalance in walletsNftBalance)
            {
                userNftToken = await loopringService.GetTokenId(settings.LoopringApiKey, responseOnAccountId.accountId, singleNftBalance.nftData);
                nftTokenId = userNftToken.data[0].tokenId;
                nftMetadataLink = await ethereumService.GetMetadataLink(userNftToken.data[0].nftId, userNftToken.data[0].tokenAddress, 0);
                nftMetadata = await nftMetadataService.GetMetadata(nftMetadataLink);
                if (nftMetadata == null)
                {

                    Console.WriteLine($"NONEFOUND {userNftToken.data[0].nftId},{responseOnAccountId.owner} {userNftToken.data[0].tokenAddress}");
                }
                else
                { 
                    Font.SetTextToGreen($"{singleNftBalance.nftData}    {singleNftBalance.total}    {nftMetadata.name}");
                }
            }
            break;
        #endregion case 4
        #region case 5
        case "5":
            Font.SetTextToBlue(menuAndUtility.allUtilities.ElementAt(5).Value);
            Font.SetTextToBlue("Here you will get all the wallet addresses that hold the given Nft Data.");
            Font.SetTextToWhite("Let's get started.");
            Font.SetTextToGreen("Is this for one or many Nfts?");
            var userResponseOnMany = Utils.CheckOneOrMany();

            if (userResponseOnMany == "one")
            {
                Font.SetTextToGreen("What is the Nft Data?");
                nftData = Utils.ReadLineWarningNoNulls("what is the Nft Data?");
                nftHoldersList = await loopringService.GetNftHoldersMultiple(settings.LoopringApiKey, nftData); 
                nftHoldersList = nftHoldersList.OrderByDescending(x=>x.amount).ToList();
                foreach (var nftHolder in nftHoldersList)
                {
                    if (nftHolder == nftHoldersList.FirstOrDefault())
                    {
                        Font.SetTextToGreen($"NftData {nftData} has {nftHoldersList.Count()} total holders.");
                        Font.SetTextToBlue($"Wallet Address \t\t\t\t\t\t Total");
                    };

                    var userAccountInformation = await loopringService.GetUserAccountInformation(nftHolder.accountId.ToString());  
                    Font.SetTextToGreen($"{userAccountInformation.owner}\t\t {nftHolder.amount}");
                }
            }
            else if (userResponseOnMany == "many")
            {
                howManyWallets = Utils.CheckInputDotTxt(fileName);
                using (StreamReader sr = new StreamReader($"./{fileName}"))
                {
                    Font.SetTextToGreen("Would you like to view just the wallet addresses?");
                    userResponseOnWalletAddressDisplay = Utils.CheckYesOrNo();
                    while ((nftData = sr.ReadLine()) != null)
                    {
                        nftHoldersList = await loopringService.GetNftHoldersMultiple(settings.LoopringApiKey, nftData); 
                        nftHoldersList = nftHoldersList.OrderByDescending(x => x.amount).ToList();
                        foreach (var nftHolder in nftHoldersList)
                        {
                            if (userResponseOnWalletAddressDisplay == "yes")
                            {
                                    var userAccountInformation = await loopringService.GetUserAccountInformation(nftHolder.accountId.ToString());  
                                    Console.WriteLine(userAccountInformation.owner);
                            }
                            else if (userResponseOnWalletAddressDisplay == "no")
                            {
                                if (nftHolder == nftHoldersList.FirstOrDefault())
                                {
                                    var minterFromNftDatas = await loopringService.GetNftInformationFromNftData(settings.LoopringApiKey, nftData);
                                    var accountIdFromMinter = await loopringService.GetUserAccountInformationFromOwner(minterFromNftDatas[0].minter);
                                    userNftToken = await loopringService.GetTokenId(settings.LoopringApiKey, accountIdFromMinter.accountId, nftData);
                                    var nftmetaDataLink = await ethereumService.GetMetadataLink(userNftToken.data[0].nftId, userNftToken.data[0].tokenAddress, 0);
                                    nftMetadata = await nftMetadataService.GetMetadata(nftmetaDataLink);

                                    Font.SetTextToGreen($"{nftMetadata.name} {nftData} has {nftHoldersList.Count()} total holders.");
                                    Font.SetTextToBlue($"Wallet Address \t\t\t\t\t\t Total");
                                };
                                
                                    var userAccountInformation = await loopringService.GetUserAccountInformation(nftHolder.accountId.ToString()); 
                                    Font.SetTextToGreen($"{userAccountInformation.owner}\t\t {nftHolder.amount}");
                            }
                        }
                    }
                }
            }
            break;
        #endregion case 5
        #region case 6
        case "6":
            Font.SetTextToBlue(menuAndUtility.allUtilities.ElementAt(6).Value);
            Font.SetTextToBlue("Here you will get all the wallet addresses that hold an account's Nfts.");
            Font.SetTextToWhite("Let's get started.");
            do
            {
                Font.SetTextToGreen("Who is the minter?");
                userResponseOnMinter = Utils.ReadLineWarningNoNulls("Who is the minter?");
                responseOnMinter = await loopringService.CheckForEthAddress(settings.LoopringApiKey, userResponseOnMinter);
                responseOnAccountId = await loopringService.GetUserAccountInformationFromOwner(responseOnMinter);
            }
            while (responseOnAccountId == null);
            Font.SetTextToGreen("Would you like to view just the wallet addresses?");
            userResponseOnWalletAddressDisplay = Utils.CheckYesOrNo();
            userMintsAndTotalList = await loopringService.GetUserMintedNfts(settings.LoopringApiKey, responseOnAccountId.accountId);
            Font.SetTextToGreen($"{userResponseOnMinter} has {userMintsAndTotalList.First().totalNum} mints");
            if (userResponseOnWalletAddressDisplay == "yes")
            {
                foreach (var userMintsAndTotalSingle in userMintsAndTotalList)
                {
                    var mints = userMintsAndTotalSingle.mints;
                    foreach (var mint in mints)
                    {
                        nftData = mint.nftData;
                        var nftHoldersAndTotal = await loopringService.GetNftHolders(settings.LoopringApiKey, nftData);

                        foreach (var item in nftHoldersAndTotal.nftHolders)
                        {
                            var userAccountInformation = await loopringService.GetUserAccountInformation(item.accountId.ToString());
                            Font.SetTextToGreen($"{userAccountInformation.owner}");
                        }
                    }
                }
            }
            else if (userResponseOnWalletAddressDisplay == "no")
            {        
                foreach (var userMintsAndTotalSingle in userMintsAndTotalList)
                {
                    var mints = userMintsAndTotalSingle.mints;
                    foreach (var mint in mints)
                    {
                        nftData = mint.nftData;
                        var nftHoldersAndTotal = await loopringService.GetNftHolders(settings.LoopringApiKey, nftData);

                        userNftToken = await loopringService.GetTokenId(settings.LoopringApiKey, mint.accountId, nftData);
                        nftMetadataLink = await ethereumService.GetMetadataLink(userNftToken.data[0].nftId, userNftToken.data[0].tokenAddress, 0);
                        nftMetadata = await nftMetadataService.GetMetadata(nftMetadataLink);

                        if (nftMetadata != null)
                        {
                            Font.SetTextToBlue($"{nftMetadata.name}");
                            Font.SetTextToBlue($"{nftMetadata.description}");
                        }
                        
                        Font.SetTextToBlueInline($"NftData: ");
                        Font.SetTextToGreen(nftData);
                        Font.SetTextToBlueInline($"Total Holders: ");
                        Font.SetTextToGreen(nftHoldersAndTotal.totalNum.ToString());
                        Font.SetTextToBlue($"Wallet Address \t\t\t\t\t\t Total");
                        foreach (var item in nftHoldersAndTotal.nftHolders)
                        {
                            var userAccountInformation = await loopringService.GetUserAccountInformation(item.accountId.ToString());
                            Font.SetTextToGreen($"{userAccountInformation.owner}\t\t {item.amount}");
                        }
                        Font.SetTextToWhite("------------------------------------------");
                    }
                }
            }
            break;
        #endregion case 6
        #region case 7
        case "7":
            var originalalletHolderList = new List<NftHolder>();
            var filteredWallettHolderList = new List<NftHolder>();
            var howManyNfts = Utils.CheckInputDotTxt(fileName);
            var allNftData = new List<string>();

            Font.SetTextToBlue(menuAndUtility.allUtilities.ElementAt(7).Value);
            Font.SetTextToBlue("Here you will provide the Nft Data and get the Nft Holders who own all Nfts provided.");
            Font.SetTextToWhite("Let's get started.");

            using (StreamReader sr = new StreamReader("./Input.txt"))
            {
                while ((nftData = sr.ReadLine()) != null)
                {
                    allNftData.Add(nftData);
                }
            }

            //Font.SetTextToPurple($"You provided the following {howManyNfts} Nfts.");

            foreach (var data in allNftData)
            {
                originalalletHolderList.AddRange(await loopringService.GetNftHoldersMultiple(settings.LoopringApiKey, data));
            }
            
            foreach (var item in originalalletHolderList.DistinctBy(x=>x.accountId))
            {
                if (originalalletHolderList.Where(x=>x.accountId == item.accountId).Count() == howManyNfts)
                {
                    filteredWallettHolderList.Add(item);
                }
            }

            Font.SetTextToPurple($"The following {filteredWallettHolderList.Count()} wallets have all above Nfts. ");
            foreach (var wallet in filteredWallettHolderList)
            {
                var walletAddress = await loopringService.GetUserAccountInformation(wallet.accountId.ToString());
                Console.WriteLine($"{walletAddress.owner}");
            }

            break;
        #endregion case 7
        #region case 8
        case "8":
            string transferMemo = "";
            Font.SetTextToBlue(menuAndUtility.allUtilities.ElementAt(8).Value);
            Font.SetTextToBlue("Here you will drop one Nft to many users.");
            Font.SetTextToWhite("Let's get started.");
            howManyWallets = Utils.CheckInputDotTxt(fileName);
            Font.SetTextToGreen($"You will be transfering to {howManyWallets} wallets.");
            Font.SetTextToBlueInline("Do you know your Nft's Nft Data?");
            Font.SetTextToYellow(" This is not the same as the Nft Id.");
            userResponseOnNftData = Utils.CheckYesOrNo();
            if (userResponseOnNftData == "yes")
            {
                Font.SetTextToBlue("Enter the NftData");
                nftData = Console.ReadLine();
                try
                {
                    userNftToken = await loopringService.GetTokenIdWithCheck(settings.LoopringApiKey, settings.LoopringAccountId, nftData);
                    nftTokenId = userNftToken.data[0].tokenId;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                Font.SetTextToBlue("Find your Nft on lexplorer.io or explorer.loopring.io.");
                Console.WriteLine("You should see the Nft Id, Minter, and Token/Collection Address");
                do
                {
                    Font.SetTextToBlue("Enter in the Nft Id");
                    string nftId = Utils.ReadLineWarningNoNulls("Enter in the Nft Id");
                    minterAndCollection = UtilsLoopring.GetMinterAndCollection();
                    minterAndCollection.minter = await loopringService.CheckForEthAddress(settings.LoopringApiKey, minterAndCollection.minter);
                    nftdataRequest = await loopringService.GetNftData(settings.LoopringApiKey, nftId, minterAndCollection.minter, minterAndCollection.TokenId);
                } while (nftdataRequest == null);
                nftData = nftdataRequest.nftData;

                userNftToken = await loopringService.GetTokenId(settings.LoopringApiKey, settings.LoopringAccountId, nftData);
                nftTokenId = userNftToken.data[0].tokenId;

            }
            nftMetadataLink = await ethereumService.GetMetadataLink(userNftToken.data[0].nftId, userNftToken.data[0].tokenAddress, 0);
            nftMetadata = await nftMetadataService.GetMetadata(nftMetadataLink);

            Font.SetTextToBlue($"How many of '{nftMetadata.name}' do you want to transfer to each address?");
            nftAmount = Utils.CheckNftSendAmount(howManyWallets, userNftToken.data[0].total, fileName);

            Font.SetTextToBlue("Memo for transfer?");
            transferMemo = Console.ReadLine()?.Trim();
            airdropNumberOn = 0;
            Font.SetTextToBlue("Starting airdrop...");
            using (StreamReader sr = new StreamReader($"./{fileName}"))
            {
                while ((toAddressInitial = sr.ReadLine()) != null)
                {
                    Console.WriteLine($"{++airdropNumberOn}/{howManyWallets}");
                    //remove whitespace after wallet address if it exists.
                    var toAddress = toAddressInitial.ToLower().Trim();

                    //Storage id
                    var storageId = await loopringService.GetNextStorageId(loopringApiKey, fromAccountId, nftTokenId);
                    // Console.WriteLine($"Storage id: {JsonConvert.SerializeObject(storageId, Formatting.Indented)}");

                    //Getting the offchain fee
                    var offChainFee = await loopringService.GetOffChainFee(loopringApiKey, fromAccountId, 11, "0");
                    // Console.WriteLine($"Offchain fee: {JsonConvert.SerializeObject(offChainFee, Formatting.Indented)}");
                    
                    //check for ens and convert to long wallet address if so
                    if (toAddress.Contains(".eth"))
                    {
                        var varHexAddress = await loopringService.GetHexAddress(settings.LoopringApiKey, toAddress);
                        if (!String.IsNullOrEmpty(varHexAddress.data))
                        {
                            toAddress = varHexAddress.data.ToLower().Trim();
                        }
                        else
                        {
                            invalidAddress.Add(toAddressInitial);
                            Thread.Sleep(100); //for a rate limiter just incase multiple invalid ens
                            continue;
                        }
                    }

                    contains = await loopringService.CheckBanishTextFile(toAddressInitial, toAddress, settings.LoopringApiKey);
                    if (contains == true)
                    {
                        banishAddress.Add(toAddressInitial);
                        continue;
                    }

                    //Calculate eddsa signautre
                    BigInteger[] poseidonInputs =
            {
                                    Utils.ParseHexUnsigned(exchange),
                                    (BigInteger) fromAccountId,
                                    (BigInteger) toAccountId,
                                    (BigInteger) nftTokenId,
                                    BigInteger.Parse(nftAmount),
                                    (BigInteger) maxFeeTokenId,
                                    BigInteger.Parse(offChainFee.fees[maxFeeTokenId].fee),
                                    Utils.ParseHexUnsigned(toAddress),
                                    (BigInteger) 0,
                                    (BigInteger) 0,
                                    (BigInteger) validUntil,
                                    (BigInteger) storageId.offchainId
                    };
                    Poseidon poseidon = new Poseidon(13, 6, 53, "poseidon", 5, _securityTarget: 128);
                    BigInteger poseidonHash = poseidon.CalculatePoseidonHash(poseidonInputs);
                    Eddsa eddsa = new Eddsa(poseidonHash, loopringPrivateKey);
                    string eddsaSignature = eddsa.Sign();

                    //Calculate ecdsa
                    string primaryTypeName = "Transfer";
                    TypedData eip712TypedData = new TypedData();
                    eip712TypedData.Domain = new Domain()
                    {
                        Name = "Loopring Protocol",
                        Version = "3.6.0",
                        ChainId = 1,
                        VerifyingContract = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                    };
                    eip712TypedData.PrimaryType = primaryTypeName;
                    eip712TypedData.Types = new Dictionary<string, MemberDescription[]>()
                    {
                        ["EIP712Domain"] = new[]
                            {
                                            new MemberDescription {Name = "name", Type = "string"},
                                            new MemberDescription {Name = "version", Type = "string"},
                                            new MemberDescription {Name = "chainId", Type = "uint256"},
                                            new MemberDescription {Name = "verifyingContract", Type = "address"},
                                        },
                        [primaryTypeName] = new[]
                            {
                                            new MemberDescription {Name = "from", Type = "address"},            // payerAddr
                                            new MemberDescription {Name = "to", Type = "address"},              // toAddr
                                            new MemberDescription {Name = "tokenID", Type = "uint16"},          // token.tokenId 
                                            new MemberDescription {Name = "amount", Type = "uint96"},           // token.volume 
                                            new MemberDescription {Name = "feeTokenID", Type = "uint16"},       // maxFee.tokenId
                                            new MemberDescription {Name = "maxFee", Type = "uint96"},           // maxFee.volume
                                            new MemberDescription {Name = "validUntil", Type = "uint32"},       // validUntill
                                            new MemberDescription {Name = "storageID", Type = "uint32"}         // storageId
                                        },

                    };
                    eip712TypedData.Message = new[]
                    {
                                    new MemberValue {TypeName = "address", Value = fromAddress},
                                    new MemberValue {TypeName = "address", Value = toAddress},
                                    new MemberValue {TypeName = "uint16", Value = nftTokenId},
                                    new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(nftAmount)},
                                    new MemberValue {TypeName = "uint16", Value = maxFeeTokenId},
                                    new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(offChainFee.fees[maxFeeTokenId].fee)},
                                    new MemberValue {TypeName = "uint32", Value = validUntil},
                                    new MemberValue {TypeName = "uint32", Value = storageId.offchainId},
                                };

                    TransferTypedData typedData = new TransferTypedData()
                    {
                        domain = new TransferTypedData.Domain()
                        {
                            name = "Loopring Protocol",
                            version = "3.6.0",
                            chainId = 1,
                            verifyingContract = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                        },
                        message = new TransferTypedData.Message()
                        {
                            from = fromAddress,
                            to = toAddress,
                            tokenID = nftTokenId,
                            amount = nftAmount,
                            feeTokenID = maxFeeTokenId,
                            maxFee = offChainFee.fees[maxFeeTokenId].fee,
                            validUntil = (int)validUntil,
                            storageID = storageId.offchainId
                        },
                        primaryType = primaryTypeName,
                        types = new TransferTypedData.Types()
                        {
                            EIP712Domain = new List<Type>()
                                        {
                                            new Type(){ name = "name", type = "string"},
                                            new Type(){ name="version", type = "string"},
                                            new Type(){ name="chainId", type = "uint256"},
                                            new Type(){ name="verifyingContract", type = "address"},
                                        },
                            Transfer = new List<Type>()
                                        {
                                            new Type(){ name = "from", type = "address"},
                                            new Type(){ name = "to", type = "address"},
                                            new Type(){ name = "tokenID", type = "uint16"},
                                            new Type(){ name = "amount", type = "uint96"},
                                            new Type(){ name = "feeTokenID", type = "uint16"},
                                            new Type(){ name = "maxFee", type = "uint96"},
                                            new Type(){ name = "validUntil", type = "uint32"},
                                            new Type(){ name = "storageID", type = "uint32"},
                                        }
                        }
                    };

                    Eip712TypedDataSigner signer = new Eip712TypedDataSigner();
                    var ethECKey = new Nethereum.Signer.EthECKey(MMorGMEPrivateKey.Replace("0x", ""));
                    var encodedTypedData = signer.EncodeTypedData(eip712TypedData);
                    var ECDRSASignature = ethECKey.SignAndCalculateV(Sha3Keccack.Current.CalculateHash(encodedTypedData));
                    var serializedECDRSASignature = EthECDSASignature.CreateStringSignature(ECDRSASignature);
                    var ecdsaSignature = serializedECDRSASignature + "0" + (int)2;

                    //Submit nft transfer
                    var nftTransferResponse = await loopringService.SubmitNftTransfer(
                        apiKey: loopringApiKey,
                        exchange: exchange,
                        fromAccountId: fromAccountId,
                        fromAddress: fromAddress,
                        toAccountId: toAccountId,
                        toAddress: toAddress,
                        nftTokenId: nftTokenId,
                        nftAmount: nftAmount,
                        maxFeeTokenId: maxFeeTokenId,
                        maxFeeAmount: offChainFee.fees[maxFeeTokenId].fee,
                        storageId.offchainId,
                        validUntil: validUntil,
                        eddsaSignature: eddsaSignature,
                        ecdsaSignature: ecdsaSignature,
                        nftData: nftData,
                        transferMemo: transferMemo
                        );

                    // Console.WriteLine(nftTransferResponse);
                    validAddress.Add(toAddressInitial);

                }
                Font.SetTextToBlue("Airdrop finished...");
                Utils.ShowAirdropAudit(validAddress, invalidAddress, banishAddress, nftMetadata.name);
            }
            break;
        #endregion case 8
        #region case 9
        case "9":
            Font.SetTextToBlue(menuAndUtility.allUtilities.ElementAt(9).Value);
            Font.SetTextToWhite("Let's get started.");
            howManyWallets = Utils.CheckInputDotTxt(fileName);
            Font.SetTextToGreen($"You will be transfering to {howManyWallets} wallets.");
            Font.SetTextToBlueInline("Do you know your Nft's Nft Data?");
            Font.SetTextToYellow(" This is not the same as the Nft Id.");
            userResponseOnNftData = Utils.CheckYesOrNo();
            if (userResponseOnNftData == "yes")
            {
                Font.SetTextToBlue("Enter the NftData");
                nftData = Console.ReadLine();
                try
                {
                    userNftToken = await loopringService.GetTokenIdWithCheck(settings.LoopringApiKey, settings.LoopringAccountId, nftData);
                    nftTokenId = userNftToken.data[0].tokenId;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            {
                Font.SetTextToBlue("Find your Nft on lexplorer.io or explorer.loopring.io.");
                Console.WriteLine("You should see the Nft Id, Minter, and Token/Collection Address");
                do
                {
                    Font.SetTextToBlue("Enter in the Nft Id");
                    string nftId = Utils.ReadLineWarningNoNulls("Enter in the Nft Id");
                    minterAndCollection = UtilsLoopring.GetMinterAndCollection();
                    minterAndCollection.minter = await loopringService.CheckForEthAddress(settings.LoopringApiKey, minterAndCollection.minter);
                    nftdataRequest = await loopringService.GetNftData(settings.LoopringApiKey, nftId, minterAndCollection.minter, minterAndCollection.TokenId);
                } while (nftdataRequest == null);
                nftData = nftdataRequest.nftData;

                userNftToken = await loopringService.GetTokenId(settings.LoopringApiKey, settings.LoopringAccountId, nftData);
                nftTokenId = userNftToken.data[0].tokenId;

            }
            nftMetadataLink = await ethereumService.GetMetadataLink(userNftToken.data[0].nftId, userNftToken.data[0].tokenAddress, 0);
            nftMetadata = await nftMetadataService.GetMetadata(nftMetadataLink);

            Font.SetTextToBlue("Memo for transfer?");
            transferMemo = Console.ReadLine()?.Trim();
            airdropNumberOn = 0;
            Font.SetTextToBlue("Starting airdrop...");
            using (StreamReader sr = new StreamReader($"./{fileName}"))
            {
                while ((toAddressInitialAndAmount = sr.ReadLine()) != null)
                {
                    string[] toAddressInitialAndAmountArray = toAddressInitialAndAmount.Split(',');
                    toAddressInitial = toAddressInitialAndAmountArray[0].Trim();
                    var toAddress = toAddressInitial.ToLower().Trim();
                    nftAmount = toAddressInitialAndAmountArray[1].Trim();

                    Console.WriteLine($"{++airdropNumberOn}/{howManyWallets}");
                    //Storage id
                    var storageId = await loopringService.GetNextStorageId(loopringApiKey, fromAccountId, nftTokenId);
                    // Console.WriteLine($"Storage id: {JsonConvert.SerializeObject(storageId, Formatting.Indented)}");

                    //Getting the offchain fee
                    var offChainFee = await loopringService.GetOffChainFee(loopringApiKey, fromAccountId, 11, "0");
                    // Console.WriteLine($"Offchain fee: {JsonConvert.SerializeObject(offChainFee, Formatting.Indented)}");

                    //check for ens and convert to long wallet address if so
                    if (toAddress.Contains(".eth"))
                    {
                        var varHexAddress = await loopringService.GetHexAddress(settings.LoopringApiKey, toAddress);
                        if (!String.IsNullOrEmpty(varHexAddress.data))
                        {
                            toAddress = varHexAddress.data.ToLower().Trim();
                        }
                        else
                        {
                            invalidAddress.Add(toAddressInitial);
                            Thread.Sleep(100); //for a rate limiter just incase multiple invalid ens
                            continue;
                        }
                    }

                    contains = await loopringService.CheckBanishTextFile(toAddressInitial, toAddress, settings.LoopringApiKey);
                    if (contains == true)
                    {
                        banishAddress.Add(toAddressInitial);
                        continue;
                    }

                    //Calculate eddsa signautre
                    BigInteger[] poseidonInputs =
            {
                                    Utils.ParseHexUnsigned(exchange),
                                    (BigInteger) fromAccountId,
                                    (BigInteger) toAccountId,
                                    (BigInteger) nftTokenId,
                                    BigInteger.Parse(nftAmount),
                                    (BigInteger) maxFeeTokenId,
                                    BigInteger.Parse(offChainFee.fees[maxFeeTokenId].fee),
                                    Utils.ParseHexUnsigned(toAddress),
                                    (BigInteger) 0,
                                    (BigInteger) 0,
                                    (BigInteger) validUntil,
                                    (BigInteger) storageId.offchainId
                    };
                    Poseidon poseidon = new Poseidon(13, 6, 53, "poseidon", 5, _securityTarget: 128);
                    BigInteger poseidonHash = poseidon.CalculatePoseidonHash(poseidonInputs);
                    Eddsa eddsa = new Eddsa(poseidonHash, loopringPrivateKey);
                    string eddsaSignature = eddsa.Sign();

                    //Calculate ecdsa
                    string primaryTypeName = "Transfer";
                    TypedData eip712TypedData = new TypedData();
                    eip712TypedData.Domain = new Domain()
                    {
                        Name = "Loopring Protocol",
                        Version = "3.6.0",
                        ChainId = 1,
                        VerifyingContract = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                    };
                    eip712TypedData.PrimaryType = primaryTypeName;
                    eip712TypedData.Types = new Dictionary<string, MemberDescription[]>()
                    {
                        ["EIP712Domain"] = new[]
                            {
                                            new MemberDescription {Name = "name", Type = "string"},
                                            new MemberDescription {Name = "version", Type = "string"},
                                            new MemberDescription {Name = "chainId", Type = "uint256"},
                                            new MemberDescription {Name = "verifyingContract", Type = "address"},
                                        },
                        [primaryTypeName] = new[]
                            {
                                            new MemberDescription {Name = "from", Type = "address"},            // payerAddr
                                            new MemberDescription {Name = "to", Type = "address"},              // toAddr
                                            new MemberDescription {Name = "tokenID", Type = "uint16"},          // token.tokenId 
                                            new MemberDescription {Name = "amount", Type = "uint96"},           // token.volume 
                                            new MemberDescription {Name = "feeTokenID", Type = "uint16"},       // maxFee.tokenId
                                            new MemberDescription {Name = "maxFee", Type = "uint96"},           // maxFee.volume
                                            new MemberDescription {Name = "validUntil", Type = "uint32"},       // validUntill
                                            new MemberDescription {Name = "storageID", Type = "uint32"}         // storageId
                                        },

                    };
                    eip712TypedData.Message = new[]
                    {
                                    new MemberValue {TypeName = "address", Value = fromAddress},
                                    new MemberValue {TypeName = "address", Value = toAddress},
                                    new MemberValue {TypeName = "uint16", Value = nftTokenId},
                                    new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(nftAmount)},
                                    new MemberValue {TypeName = "uint16", Value = maxFeeTokenId},
                                    new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(offChainFee.fees[maxFeeTokenId].fee)},
                                    new MemberValue {TypeName = "uint32", Value = validUntil},
                                    new MemberValue {TypeName = "uint32", Value = storageId.offchainId},
                                };

                    TransferTypedData typedData = new TransferTypedData()
                    {
                        domain = new TransferTypedData.Domain()
                        {
                            name = "Loopring Protocol",
                            version = "3.6.0",
                            chainId = 1,
                            verifyingContract = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                        },
                        message = new TransferTypedData.Message()
                        {
                            from = fromAddress,
                            to = toAddress,
                            tokenID = nftTokenId,
                            amount = nftAmount,
                            feeTokenID = maxFeeTokenId,
                            maxFee = offChainFee.fees[maxFeeTokenId].fee,
                            validUntil = (int)validUntil,
                            storageID = storageId.offchainId
                        },
                        primaryType = primaryTypeName,
                        types = new TransferTypedData.Types()
                        {
                            EIP712Domain = new List<Type>()
                                        {
                                            new Type(){ name = "name", type = "string"},
                                            new Type(){ name="version", type = "string"},
                                            new Type(){ name="chainId", type = "uint256"},
                                            new Type(){ name="verifyingContract", type = "address"},
                                        },
                            Transfer = new List<Type>()
                                        {
                                            new Type(){ name = "from", type = "address"},
                                            new Type(){ name = "to", type = "address"},
                                            new Type(){ name = "tokenID", type = "uint16"},
                                            new Type(){ name = "amount", type = "uint96"},
                                            new Type(){ name = "feeTokenID", type = "uint16"},
                                            new Type(){ name = "maxFee", type = "uint96"},
                                            new Type(){ name = "validUntil", type = "uint32"},
                                            new Type(){ name = "storageID", type = "uint32"},
                                        }
                        }
                    };

                    Eip712TypedDataSigner signer = new Eip712TypedDataSigner();
                    var ethECKey = new Nethereum.Signer.EthECKey(MMorGMEPrivateKey.Replace("0x", ""));
                    var encodedTypedData = signer.EncodeTypedData(eip712TypedData);
                    var ECDRSASignature = ethECKey.SignAndCalculateV(Sha3Keccack.Current.CalculateHash(encodedTypedData));
                    var serializedECDRSASignature = EthECDSASignature.CreateStringSignature(ECDRSASignature);
                    var ecdsaSignature = serializedECDRSASignature + "0" + (int)2;

                    //Submit nft transfer
                    var nftTransferResponse = await loopringService.SubmitNftTransfer(
                        apiKey: loopringApiKey,
                        exchange: exchange,
                        fromAccountId: fromAccountId,
                        fromAddress: fromAddress,
                        toAccountId: toAccountId,
                        toAddress: toAddress,
                        nftTokenId: nftTokenId,
                        nftAmount: nftAmount,
                        maxFeeTokenId: maxFeeTokenId,
                        maxFeeAmount: offChainFee.fees[maxFeeTokenId].fee,
                        storageId.offchainId,
                        validUntil: validUntil,
                        eddsaSignature: eddsaSignature,
                        ecdsaSignature: ecdsaSignature,
                        nftData: nftData,
                        transferMemo: transferMemo
                        );

                    // Console.WriteLine(nftTransferResponse);
                    validAddress.Add(toAddressInitial);

                }
                Font.SetTextToBlue("Airdrop finished...");
                Utils.ShowAirdropAudit(validAddress, invalidAddress, banishAddress, nftMetadata.name);
            }
            break;
        #endregion case 9
        #region case 10
        case "10":
            Font.SetTextToBlue(menuAndUtility.allUtilities.ElementAt(10).Value);
            Font.SetTextToBlue($"Here you will drop many Nfts to many users.");
            Font.SetTextToWhite("Let's get started.");
            howManyWallets = Utils.CheckInputDotTxtTwoInputs(fileName);
            Font.SetTextToGreen($"You will be transfering to {howManyWallets} wallets.");
            Font.SetTextToBlue("How many of each Nft do you want to transfer to each address?");
            nftAmount = Utils.ReadLineWarningNoNullsForceInt("How many of each Nft do you want to transfer to each address?");

            Font.SetTextToBlue("Memo for transfer?");
            transferMemo = Console.ReadLine()?.Trim();

            string walletAddressLine;
            Font.SetTextToBlue("Starting airdrop...");
            airdropNumberOn = 0;
            using (StreamReader sr = new StreamReader($"./{fileName}"))
            {
                while ((walletAddressLine = sr.ReadLine()) != null)
                {
                    string[] walletAddressLineArray = walletAddressLine.Split(',');
                    toAddressInitial = walletAddressLineArray[0].Trim();
                    var toAddress = toAddressInitial.ToLower().Trim();
                    nftData = walletAddressLineArray[1].ToLower().Trim();

                    userNftToken = await loopringService.GetTokenId(settings.LoopringApiKey, settings.LoopringAccountId, nftData);  //the nft tokenId, not the nftId
                    if (userNftToken.totalNum == 0)
                    {
                        invalidAddress.Add(walletAddressLine);
                        Thread.Sleep(100); //for a rate limiter just incase multiple invalid ens
                        continue;
                    }
                    nftTokenId = userNftToken.data[0].tokenId;

                    Console.WriteLine($"{++airdropNumberOn}/{howManyWallets}");

                    //Storage id
                    var storageId = await loopringService.GetNextStorageId(loopringApiKey, fromAccountId, nftTokenId);
                    // Console.WriteLine($"Storage id: {JsonConvert.SerializeObject(storageId, Formatting.Indented)}");

                    //Getting the offchain fee
                    var offChainFee = await loopringService.GetOffChainFee(loopringApiKey, fromAccountId, 11, "0");
                    // Console.WriteLine($"Offchain fee: {JsonConvert.SerializeObject(offChainFee, Formatting.Indented)}");

                    //check for ens and convert to long wallet address if so
                    if (toAddress.Contains(".eth"))
                    {
                        var varHexAddress = await loopringService.GetHexAddress(settings.LoopringApiKey, toAddress);
                        if (!String.IsNullOrEmpty(varHexAddress.data))
                        {
                            toAddress = varHexAddress.data;
                        }
                        else
                        {
                            invalidAddress.Add(toAddress);
                            Thread.Sleep(100); //for a rate limiter just incase multiple invalid ens
                            continue;
                        }
                    }

                    contains = await loopringService.CheckBanishTextFile(toAddressInitial, toAddress, settings.LoopringApiKey);
                    if (contains == true)
                    {
                        banishAddress.Add(toAddressInitial);
                        continue;
                    }

                    //Calculate eddsa signautre
                    BigInteger[] poseidonInputs =
            {
                                    Utils.ParseHexUnsigned(exchange),
                                    (BigInteger) fromAccountId,
                                    (BigInteger) toAccountId,
                                    (BigInteger) nftTokenId,
                                    BigInteger.Parse(nftAmount),
                                    (BigInteger) maxFeeTokenId,
                                    BigInteger.Parse(offChainFee.fees[maxFeeTokenId].fee),
                                    Utils.ParseHexUnsigned(toAddress),
                                    (BigInteger) 0,
                                    (BigInteger) 0,
                                    (BigInteger) validUntil,
                                    (BigInteger) storageId.offchainId
                    };
                    Poseidon poseidon = new Poseidon(13, 6, 53, "poseidon", 5, _securityTarget: 128);
                    BigInteger poseidonHash = poseidon.CalculatePoseidonHash(poseidonInputs);
                    Eddsa eddsa = new Eddsa(poseidonHash, loopringPrivateKey);
                    string eddsaSignature = eddsa.Sign();

                    //Calculate ecdsa
                    string primaryTypeName = "Transfer";
                    TypedData eip712TypedData = new TypedData();
                    eip712TypedData.Domain = new Domain()
                    {
                        Name = "Loopring Protocol",
                        Version = "3.6.0",
                        ChainId = 1,
                        VerifyingContract = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                    };
                    eip712TypedData.PrimaryType = primaryTypeName;
                    eip712TypedData.Types = new Dictionary<string, MemberDescription[]>()
                    {
                        ["EIP712Domain"] = new[]
                            {
                                            new MemberDescription {Name = "name", Type = "string"},
                                            new MemberDescription {Name = "version", Type = "string"},
                                            new MemberDescription {Name = "chainId", Type = "uint256"},
                                            new MemberDescription {Name = "verifyingContract", Type = "address"},
                                        },
                        [primaryTypeName] = new[]
                            {
                                            new MemberDescription {Name = "from", Type = "address"},            // payerAddr
                                            new MemberDescription {Name = "to", Type = "address"},              // toAddr
                                            new MemberDescription {Name = "tokenID", Type = "uint16"},          // token.tokenId 
                                            new MemberDescription {Name = "amount", Type = "uint96"},           // token.volume 
                                            new MemberDescription {Name = "feeTokenID", Type = "uint16"},       // maxFee.tokenId
                                            new MemberDescription {Name = "maxFee", Type = "uint96"},           // maxFee.volume
                                            new MemberDescription {Name = "validUntil", Type = "uint32"},       // validUntill
                                            new MemberDescription {Name = "storageID", Type = "uint32"}         // storageId
                                        },

                    };
                    eip712TypedData.Message = new[]
                    {
                                    new MemberValue {TypeName = "address", Value = fromAddress},
                                    new MemberValue {TypeName = "address", Value = toAddress},
                                    new MemberValue {TypeName = "uint16", Value = nftTokenId},
                                    new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(nftAmount)},
                                    new MemberValue {TypeName = "uint16", Value = maxFeeTokenId},
                                    new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(offChainFee.fees[maxFeeTokenId].fee)},
                                    new MemberValue {TypeName = "uint32", Value = validUntil},
                                    new MemberValue {TypeName = "uint32", Value = storageId.offchainId},
                                };

                    TransferTypedData typedData = new TransferTypedData()
                    {
                        domain = new TransferTypedData.Domain()
                        {
                            name = "Loopring Protocol",
                            version = "3.6.0",
                            chainId = 1,
                            verifyingContract = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                        },
                        message = new TransferTypedData.Message()
                        {
                            from = fromAddress,
                            to = toAddress,
                            tokenID = nftTokenId,
                            amount = nftAmount,
                            feeTokenID = maxFeeTokenId,
                            maxFee = offChainFee.fees[maxFeeTokenId].fee,
                            validUntil = (int)validUntil,
                            storageID = storageId.offchainId
                        },
                        primaryType = primaryTypeName,
                        types = new TransferTypedData.Types()
                        {
                            EIP712Domain = new List<Type>()
                                        {
                                            new Type(){ name = "name", type = "string"},
                                            new Type(){ name="version", type = "string"},
                                            new Type(){ name="chainId", type = "uint256"},
                                            new Type(){ name="verifyingContract", type = "address"},
                                        },
                            Transfer = new List<Type>()
                                        {
                                            new Type(){ name = "from", type = "address"},
                                            new Type(){ name = "to", type = "address"},
                                            new Type(){ name = "tokenID", type = "uint16"},
                                            new Type(){ name = "amount", type = "uint96"},
                                            new Type(){ name = "feeTokenID", type = "uint16"},
                                            new Type(){ name = "maxFee", type = "uint96"},
                                            new Type(){ name = "validUntil", type = "uint32"},
                                            new Type(){ name = "storageID", type = "uint32"},
                                        }
                        }
                    };

                    Eip712TypedDataSigner signer = new Eip712TypedDataSigner();
                    var ethECKey = new Nethereum.Signer.EthECKey(MMorGMEPrivateKey.Replace("0x", ""));
                    var encodedTypedData = signer.EncodeTypedData(eip712TypedData);
                    var ECDRSASignature = ethECKey.SignAndCalculateV(Sha3Keccack.Current.CalculateHash(encodedTypedData));
                    var serializedECDRSASignature = EthECDSASignature.CreateStringSignature(ECDRSASignature);
                    var ecdsaSignature = serializedECDRSASignature + "0" + (int)2;

                    //Submit nft transfer
                    var nftTransferResponse = await loopringService.SubmitNftTransfer(
                        apiKey: loopringApiKey,
                        exchange: exchange,
                        fromAccountId: fromAccountId,
                        fromAddress: fromAddress,
                        toAccountId: toAccountId,
                        toAddress: toAddress,
                        nftTokenId: nftTokenId,
                        nftAmount: nftAmount,
                        maxFeeTokenId: maxFeeTokenId,
                        maxFeeAmount: offChainFee.fees[maxFeeTokenId].fee,
                        storageId.offchainId,
                        validUntil: validUntil,
                        eddsaSignature: eddsaSignature,
                        ecdsaSignature: ecdsaSignature,
                        nftData: nftData,
                        transferMemo: transferMemo
                        );

                    Console.WriteLine(nftTransferResponse);
                    validAddress.Add(toAddressInitial);

                }
                Font.SetTextToBlue("Airdrop finished...");
                Utils.ShowAirdropAuditAmbiguous(validAddress, invalidAddress, banishAddress);
            }
            break;
        #endregion case 10
        #region case 11
        case "11":
            Font.SetTextToBlue(menuAndUtility.allUtilities.ElementAt(11).Value);
            Font.SetTextToBlue("Here you will send all Nfts owned by those in the banish file to the dead address.");
            Font.SetTextToWhite("Starting...");

            var walletsNfts = await loopringService.GetWalletsNfts(loopringApiKey, fromAccountId);
            var banishedNfts = new List<Datum>();
            foreach (var nft in walletsNfts)
            {
                var nftInformation = await loopringService.GetNftInformationFromNftData(loopringApiKey, nft.nftData);
                contains = await loopringService.CheckBanishFile(settings.LoopringApiKey, nftInformation.FirstOrDefault().minter.ToLower());
                if (contains == true)
                {
                    // make a list to show to the user before sending. show them who minted, nft amout, nft name
                    banishedNfts.Add(nft);
                }
            }
            foreach (var nft in banishedNfts)
            {
                nftMetadataLink = await ethereumService.GetMetadataLink(nft.nftId, nft.tokenAddress, 0);
                nftMetadata = await nftMetadataService.GetMetadata(nftMetadataLink);
                Console.WriteLine($"{nftMetadata.name} will be transfered.");
            }
            Font.SetTextToGreen("Would you like to send the above Nfts to the dead Address?");
            userResponseOnWalletAddressDisplay = Utils.CheckYesOrNo();
            // take the list and then send them if the user approves.
            if (userResponseOnWalletAddressDisplay == "yes")
            {
                foreach (var banishedNft in banishedNfts)
                {
                    nftTokenId = banishedNft.tokenId;
                    nftAmount = "1";

                    //Storage id
                    var storageId = await loopringService.GetNextStorageId(loopringApiKey, fromAccountId, nftTokenId);
                    // Console.WriteLine($"Storage id: {JsonConvert.SerializeObject(storageId, Formatting.Indented)}");

                    //Getting the offchain fee
                    var offChainFee = await loopringService.GetOffChainFee(loopringApiKey, fromAccountId, 11, "0");
                    // Console.WriteLine($"Offchain fee: {JsonConvert.SerializeObject(offChainFee, Formatting.Indented)}");

                    var toAddress = "0x000000000000000000000000000000000000dEaD";

                    //Calculate eddsa signautre
                    BigInteger[] poseidonInputs =
            {
                                        Utils.ParseHexUnsigned(exchange),
                                        (BigInteger) fromAccountId,
                                        (BigInteger) toAccountId,
                                        (BigInteger) nftTokenId,
                                        BigInteger.Parse(nftAmount),
                                        (BigInteger) maxFeeTokenId,
                                        BigInteger.Parse(offChainFee.fees[maxFeeTokenId].fee),
                                        Utils.ParseHexUnsigned(toAddress),
                                        (BigInteger) 0,
                                        (BigInteger) 0,
                                        (BigInteger) validUntil,
                                        (BigInteger) storageId.offchainId
                        };
                    Poseidon poseidon = new Poseidon(13, 6, 53, "poseidon", 5, _securityTarget: 128);
                    BigInteger poseidonHash = poseidon.CalculatePoseidonHash(poseidonInputs);
                    Eddsa eddsa = new Eddsa(poseidonHash, loopringPrivateKey);
                    string eddsaSignature = eddsa.Sign();

                    //Calculate ecdsa
                    string primaryTypeName = "Transfer";
                    TypedData eip712TypedData = new TypedData();
                    eip712TypedData.Domain = new Domain()
                    {
                        Name = "Loopring Protocol",
                        Version = "3.6.0",
                        ChainId = 1,
                        VerifyingContract = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                    };
                    eip712TypedData.PrimaryType = primaryTypeName;
                    eip712TypedData.Types = new Dictionary<string, MemberDescription[]>()
                    {
                        ["EIP712Domain"] = new[]
                            {
                                                new MemberDescription {Name = "name", Type = "string"},
                                                new MemberDescription {Name = "version", Type = "string"},
                                                new MemberDescription {Name = "chainId", Type = "uint256"},
                                                new MemberDescription {Name = "verifyingContract", Type = "address"},
                                            },
                        [primaryTypeName] = new[]
                            {
                                                new MemberDescription {Name = "from", Type = "address"},            // payerAddr
                                                new MemberDescription {Name = "to", Type = "address"},              // toAddr
                                                new MemberDescription {Name = "tokenID", Type = "uint16"},          // token.tokenId 
                                                new MemberDescription {Name = "amount", Type = "uint96"},           // token.volume 
                                                new MemberDescription {Name = "feeTokenID", Type = "uint16"},       // maxFee.tokenId
                                                new MemberDescription {Name = "maxFee", Type = "uint96"},           // maxFee.volume
                                                new MemberDescription {Name = "validUntil", Type = "uint32"},       // validUntill
                                                new MemberDescription {Name = "storageID", Type = "uint32"}         // storageId
                                            },

                    };
                    eip712TypedData.Message = new[]
                    {
                                        new MemberValue {TypeName = "address", Value = fromAddress},
                                        new MemberValue {TypeName = "address", Value = toAddress},
                                        new MemberValue {TypeName = "uint16", Value = nftTokenId},
                                        new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(nftAmount)},
                                        new MemberValue {TypeName = "uint16", Value = maxFeeTokenId},
                                        new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(offChainFee.fees[maxFeeTokenId].fee)},
                                        new MemberValue {TypeName = "uint32", Value = validUntil},
                                        new MemberValue {TypeName = "uint32", Value = storageId.offchainId},
                                    };

                    TransferTypedData typedData = new TransferTypedData()
                    {
                        domain = new TransferTypedData.Domain()
                        {
                            name = "Loopring Protocol",
                            version = "3.6.0",
                            chainId = 1,
                            verifyingContract = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                        },
                        message = new TransferTypedData.Message()
                        {
                            from = fromAddress,
                            to = toAddress,
                            tokenID = nftTokenId,
                            amount = nftAmount,
                            feeTokenID = maxFeeTokenId,
                            maxFee = offChainFee.fees[maxFeeTokenId].fee,
                            validUntil = (int)validUntil,
                            storageID = storageId.offchainId
                        },
                        primaryType = primaryTypeName,
                        types = new TransferTypedData.Types()
                        {
                            EIP712Domain = new List<Type>()
                                            {
                                                new Type(){ name = "name", type = "string"},
                                                new Type(){ name="version", type = "string"},
                                                new Type(){ name="chainId", type = "uint256"},
                                                new Type(){ name="verifyingContract", type = "address"},
                                            },
                            Transfer = new List<Type>()
                                            {
                                                new Type(){ name = "from", type = "address"},
                                                new Type(){ name = "to", type = "address"},
                                                new Type(){ name = "tokenID", type = "uint16"},
                                                new Type(){ name = "amount", type = "uint96"},
                                                new Type(){ name = "feeTokenID", type = "uint16"},
                                                new Type(){ name = "maxFee", type = "uint96"},
                                                new Type(){ name = "validUntil", type = "uint32"},
                                                new Type(){ name = "storageID", type = "uint32"},
                                            }
                        }
                    };

                    Eip712TypedDataSigner signer = new Eip712TypedDataSigner();
                    var ethECKey = new Nethereum.Signer.EthECKey(MMorGMEPrivateKey.Replace("0x", ""));
                    var encodedTypedData = signer.EncodeTypedData(eip712TypedData);
                    var ECDRSASignature = ethECKey.SignAndCalculateV(Sha3Keccack.Current.CalculateHash(encodedTypedData));
                    var serializedECDRSASignature = EthECDSASignature.CreateStringSignature(ECDRSASignature);
                    var ecdsaSignature = serializedECDRSASignature + "0" + (int)2;

                    //Submit nft transfer
                    var nftTransferResponse = await loopringService.SubmitNftTransfer(
                        apiKey: loopringApiKey,
                        exchange: exchange,
                        fromAccountId: fromAccountId,
                        fromAddress: fromAddress,
                        toAccountId: toAccountId,
                        toAddress: toAddress,
                        nftTokenId: nftTokenId,
                        nftAmount: nftAmount,
                        maxFeeTokenId: maxFeeTokenId,
                        maxFeeAmount: offChainFee.fees[maxFeeTokenId].fee,
                        storageId.offchainId,
                        validUntil: validUntil,
                        eddsaSignature: eddsaSignature,
                        ecdsaSignature: ecdsaSignature,
                        nftData: banishedNft.nftData,
                        transferMemo: "Sending trash Nft to Dead"
                        );

                    Console.WriteLine(nftTransferResponse);

                }
            }
            else
            {
                Console.WriteLine("Please check your banish file and restart.");
            }


            break;
        #endregion case 11
        #region case 12
        case "12":
            //Token id of 1 for LRC, token id of 0 for ETH
            decimal amountToTransfer = 0m;
            transferMemo = "";
            int transferTokenId = 3;
            string transferTokenSymbol = "";
            Font.SetTextToBlue(menuAndUtility.allUtilities.ElementAt(12).Value);
            Font.SetTextToBlue($"Here you will airdrop LRC/ETH to many users.");
            Font.SetTextToWhite("Let's get started.");
            howManyWallets = Utils.CheckInputDotTxt(fileName);
            Font.SetTextToGreen($"You will be transfering to {howManyWallets} wallets.");
            Font.SetTextToBlue("Do you want to send LRC(1) or ETH(0)?");
            transferTokenId = Utils.Check1Or2(transferTokenId);
            Font.SetTextToBlue("Amount to transfer per address?");
            amountToTransfer = Utils.ReadLineWarningNoNullsForceDecimal("Amount to transfer per address?");
            Font.SetTextToBlue("Memo for transfer?");
            transferMemo = Console.ReadLine()?.Trim();

            if (transferTokenId == 1)
            {
                transferTokenSymbol = "LRC";
            }
            else if (transferTokenId == 0)
            {
                transferTokenSymbol = "ETH";
            }

            Font.SetTextToBlue("Starting airdrop...");
            airdropNumberOn = 0;
            using (StreamReader sr = new StreamReader($"./{fileName}"))
            {
                while ((walletAddressLine = sr.ReadLine()) != null)
                {
                    toAddressInitial = walletAddressLine;
                    var transferToAddress = walletAddressLine.ToLower().Trim();

                    Console.WriteLine($"{++airdropNumberOn}/{howManyWallets}");

                    //check for ens and convert to long wallet address if so
                    if (transferToAddress.Contains(".eth"))
                    {
                        var varHexAddress = await loopringService.GetHexAddress(settings.LoopringApiKey, transferToAddress);
                        if (!String.IsNullOrEmpty(varHexAddress.data))
                        {
                            transferToAddress = varHexAddress.data;
                        }
                        else
                        {
                            invalidAddress.Add(transferToAddress);
                            Thread.Sleep(1); //for a rate limiter just incase multiple invalid ens
                            continue;
                        }
                    }

                    contains = await loopringService.CheckBanishTextFile(toAddressInitial, transferToAddress, settings.LoopringApiKey);
                    if (contains == true)
                    {
                        banishAddress.Add(toAddressInitial);
                        continue;
                    }


                    var amount = (amountToTransfer * 1000000000000000000m).ToString("0");
                    var transferFeeAmountResult = await loopringService.GetOffChainTransferFee(loopringApiKey, fromAccountId, 3, transferTokenSymbol, amount); //3 is the request type for crypto transfers
                    var feeAmount = transferFeeAmountResult.fees.Where(w => w.token == transferTokenSymbol).First().fee;
                    var transferStorageId = await loopringService.GetNextStorageId(loopringApiKey, fromAccountId, transferTokenId);

                    TransferRequest req = new TransferRequest()
                    {
                        exchange = exchange,
                        maxFee = new Token()
                        {
                            tokenId = transferTokenId,
                            volume = feeAmount
                        },
                        token = new Token()
                        {
                            tokenId = transferTokenId,
                            volume = amount
                        },
                        payeeAddr = transferToAddress,
                        payerAddr = fromAddress,
                        payeeId = 0,
                        payerId = fromAccountId,
                        storageId = transferStorageId.offchainId,
                        validUntil = Utils.GetUnixTimestamp() + (int)TimeSpan.FromDays(365).TotalSeconds,
                        tokenName = transferTokenSymbol,
                        tokenFeeName = transferTokenSymbol
                    };

                    BigInteger[] eddsaSignatureinputs = {
                Utils.ParseHexUnsigned(req.exchange),
                (BigInteger)req.payerId,
                (BigInteger)req.payeeId,
                (BigInteger)req.token.tokenId,
                BigInteger.Parse(req.token.volume),
                (BigInteger)req.maxFee.tokenId,
                BigInteger.Parse(req.maxFee.volume),
                Utils.ParseHexUnsigned(req.payeeAddr),
                0,
                0,
                (BigInteger)req.validUntil,
                (BigInteger)req.storageId
            };

                    Poseidon poseidonTransfer = new Poseidon(13, 6, 53, "poseidon", 5, _securityTarget: 128);
                    BigInteger poseidonTransferHash = poseidonTransfer.CalculatePoseidonHash(eddsaSignatureinputs);
                    Eddsa eddsaTransfer = new Eddsa(poseidonTransferHash, loopringPrivateKey);
                    string transferEddsaSignature = eddsaTransfer.Sign();

                    //Calculate ecdsa
                    string primaryTypeNameTransfer = "Transfer";
                    TypedData eip712TypedDataTransfer = new TypedData();
                    eip712TypedDataTransfer.Domain = new Domain()
                    {
                        Name = "Loopring Protocol",
                        Version = "3.6.0",
                        ChainId = 1,
                        //ChainId = 5,
                        VerifyingContract = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                        //VerifyingContract = "0x2e76EBd1c7c0C8e7c2B875b6d505a260C525d25e",
                    };
                    eip712TypedDataTransfer.PrimaryType = primaryTypeNameTransfer;
                    eip712TypedDataTransfer.Types = new Dictionary<string, MemberDescription[]>()
                    {
                        ["EIP712Domain"] = new[]
                            {
                                            new MemberDescription {Name = "name", Type = "string"},
                                            new MemberDescription {Name = "version", Type = "string"},
                                            new MemberDescription {Name = "chainId", Type = "uint256"},
                                            new MemberDescription {Name = "verifyingContract", Type = "address"},
                                        },
                        [primaryTypeNameTransfer] = new[]
                            {
                                            new MemberDescription {Name = "from", Type = "address"},            // payerAddr
                                            new MemberDescription {Name = "to", Type = "address"},              // toAddr
                                            new MemberDescription {Name = "tokenID", Type = "uint16"},          // token.tokenId 
                                            new MemberDescription {Name = "amount", Type = "uint96"},           // token.volume 
                                            new MemberDescription {Name = "feeTokenID", Type = "uint16"},       // maxFee.tokenId
                                            new MemberDescription {Name = "maxFee", Type = "uint96"},           // maxFee.volume
                                            new MemberDescription {Name = "validUntil", Type = "uint32"},       // validUntill
                                            new MemberDescription {Name = "storageID", Type = "uint32"}         // storageId
                                        },

                    };
                    eip712TypedDataTransfer.Message = new[]
                    {
                                    new MemberValue {TypeName = "address", Value = fromAddress},
                                    new MemberValue {TypeName = "address", Value = transferToAddress},
                                    new MemberValue {TypeName = "uint16", Value = req.token.tokenId},
                                    new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(req.token.volume)},
                                    new MemberValue {TypeName = "uint16", Value = req.maxFee.tokenId},
                                    new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(req.maxFee.volume)},
                                    new MemberValue {TypeName = "uint32", Value = req.validUntil},
                                    new MemberValue {TypeName = "uint32", Value = req.storageId},
                                };

                    TransferTypedData typedDataTransfer = new TransferTypedData()
                    {
                        domain = new TransferTypedData.Domain()
                        {
                            name = "Loopring Protocol",
                            version = "3.6.0",
                            chainId = 1,
                            //chainId = 5,
                            verifyingContract = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                            //verifyingContract = "0x2e76EBd1c7c0C8e7c2B875b6d505a260C525d25e",
                        },
                        message = new TransferTypedData.Message()
                        {
                            from = fromAddress,
                            to = transferToAddress,
                            tokenID = req.token.tokenId,
                            amount = req.token.volume,
                            feeTokenID = req.maxFee.tokenId,
                            maxFee = req.maxFee.volume,
                            validUntil = (int)req.validUntil,
                            storageID = req.storageId
                        },
                        primaryType = primaryTypeNameTransfer,
                        types = new TransferTypedData.Types()
                        {
                            EIP712Domain = new List<Type>()
                                        {
                                            new Type(){ name = "name", type = "string"},
                                            new Type(){ name="version", type = "string"},
                                            new Type(){ name="chainId", type = "uint256"},
                                            new Type(){ name="verifyingContract", type = "address"},
                                        },
                            Transfer = new List<Type>()
                                        {
                                            new Type(){ name = "from", type = "address"},
                                            new Type(){ name = "to", type = "address"},
                                            new Type(){ name = "tokenID", type = "uint16"},
                                            new Type(){ name = "amount", type = "uint96"},
                                            new Type(){ name = "feeTokenID", type = "uint16"},
                                            new Type(){ name = "maxFee", type = "uint96"},
                                            new Type(){ name = "validUntil", type = "uint32"},
                                            new Type(){ name = "storageID", type = "uint32"},
                                        }
                        }
                    };

                    Eip712TypedDataSigner signerTransfer = new Eip712TypedDataSigner();
                    var ethECKeyTransfer = new Nethereum.Signer.EthECKey(MMorGMEPrivateKey.Replace("0x", ""));
                    var encodedTypedDataTransfer = signerTransfer.EncodeTypedData(eip712TypedDataTransfer);
                    var ECDRSASignatureTransfer = ethECKeyTransfer.SignAndCalculateV(Sha3Keccack.Current.CalculateHash(encodedTypedDataTransfer));
                    var serializedECDRSASignatureTransfer = EthECDSASignature.CreateStringSignature(ECDRSASignatureTransfer);
                    var transferEcdsaSignature = serializedECDRSASignatureTransfer + "0" + (int)2;

                    var tokenTransferResult = await loopringService.SubmitTokenTransfer(
                        loopringApiKey,
                        exchange,
                        fromAccountId,
                        fromAddress,
                        0,
                        transferToAddress,
                        req.token.tokenId,
                        req.token.volume,
                        req.maxFee.tokenId,
                        req.maxFee.volume,
                        req.storageId,
                        req.validUntil,
                        transferEddsaSignature,
                        transferEcdsaSignature,
                        transferMemo);
                    // Console.WriteLine(tokenTransferResult);
                    validAddress.Add(transferToAddress);
                }
                Font.SetTextToBlue("Airdrop finished...");
                Utils.ShowAirdropAuditCrypto(validAddress, invalidAddress, banishAddress, transferTokenSymbol);
            }
            break;
        #endregion case 12
        #region case 13
        case "13":
            //Token id of 1 for LRC, token id of 0 for ETH
            amountToTransfer = 0m;
            transferMemo = "";
            transferTokenId = 3;
            transferTokenSymbol = "";
            Font.SetTextToBlue(menuAndUtility.allUtilities.ElementAt(13).Value);
            Font.SetTextToBlue($"Here you will airdrop LRC/ETH to many users with different amounts.");
            Font.SetTextToWhite("Let's get started.");
            howManyWallets = Utils.CheckInputDotTxt(fileName);
            Font.SetTextToGreen($"You will be transfering to {howManyWallets} wallets.");
            Font.SetTextToBlue("Do you want to send LRC(1) or ETH(0)?");
            transferTokenId = Utils.Check1Or2(transferTokenId);
            //Font.SetTextToBlue("Amount to transfer per address?");
            //amountToTransfer = Utils.ReadLineWarningNoNullsForceDecimal("Amount to transfer per address?");
            Font.SetTextToBlue("Memo for transfer?");
            transferMemo = Console.ReadLine()?.Trim();

            if (transferTokenId == 1)
            {
                transferTokenSymbol = "LRC";
            }
            else if (transferTokenId == 0)
            {
                transferTokenSymbol = "ETH";
            }

            Font.SetTextToBlue("Starting airdrop...");
            airdropNumberOn = 0;
            using (StreamReader sr = new StreamReader($"./{fileName}"))
            {
                while ((walletAddressLine = sr.ReadLine()) != null)
                {
                    string[] walletAddressLineArray = walletAddressLine.Split(',');
                    var transferToAddress = walletAddressLineArray[0].ToLower().Trim();
                    toAddressInitial = walletAddressLineArray[0].Trim();
                    amountToTransfer = decimal.Parse(walletAddressLineArray[1].ToLower().Trim());

                    Console.WriteLine($"{++airdropNumberOn}/{howManyWallets}");

                    //check for ens and convert to long wallet address if so
                    if (transferToAddress.Contains(".eth"))
                    {
                        var varHexAddress = await loopringService.GetHexAddress(settings.LoopringApiKey, transferToAddress);
                        if (!String.IsNullOrEmpty(varHexAddress.data))
                        {
                            transferToAddress = varHexAddress.data;
                        }
                        else
                        {
                            invalidAddress.Add(transferToAddress);
                            Thread.Sleep(1); //for a rate limiter just incase multiple invalid ens
                            continue;
                        }
                    }

                    contains = await loopringService.CheckBanishTextFile(toAddressInitial, transferToAddress, settings.LoopringApiKey);
                    if (contains == true)
                    {
                        banishAddress.Add(toAddressInitial);
                        continue;
                    }

                    var amount = (amountToTransfer * 1000000000000000000m).ToString("0");
                    var transferFeeAmountResult = await loopringService.GetOffChainTransferFee(loopringApiKey, fromAccountId, 3, transferTokenSymbol, amount); //3 is the request type for crypto transfers
                    var feeAmount = transferFeeAmountResult.fees.Where(w => w.token == transferTokenSymbol).First().fee;
                    var transferStorageId = await loopringService.GetNextStorageId(loopringApiKey, fromAccountId, transferTokenId);

                    TransferRequest req = new TransferRequest()
                    {
                        exchange = exchange,
                        maxFee = new Token()
                        {
                            tokenId = transferTokenId,
                            volume = feeAmount
                        },
                        token = new Token()
                        {
                            tokenId = transferTokenId,
                            volume = amount
                        },
                        payeeAddr = transferToAddress,
                        payerAddr = fromAddress,
                        payeeId = 0,
                        payerId = fromAccountId,
                        storageId = transferStorageId.offchainId,
                        validUntil = Utils.GetUnixTimestamp() + (int)TimeSpan.FromDays(365).TotalSeconds,
                        tokenName = transferTokenSymbol,
                        tokenFeeName = transferTokenSymbol
                    };

                    BigInteger[] eddsaSignatureinputs = {
                Utils.ParseHexUnsigned(req.exchange),
                (BigInteger)req.payerId,
                (BigInteger)req.payeeId,
                (BigInteger)req.token.tokenId,
                BigInteger.Parse(req.token.volume),
                (BigInteger)req.maxFee.tokenId,
                BigInteger.Parse(req.maxFee.volume),
                Utils.ParseHexUnsigned(req.payeeAddr),
                0,
                0,
                (BigInteger)req.validUntil,
                (BigInteger)req.storageId
            };

                    Poseidon poseidonTransfer = new Poseidon(13, 6, 53, "poseidon", 5, _securityTarget: 128);
                    BigInteger poseidonTransferHash = poseidonTransfer.CalculatePoseidonHash(eddsaSignatureinputs);
                    Eddsa eddsaTransfer = new Eddsa(poseidonTransferHash, loopringPrivateKey);
                    string transferEddsaSignature = eddsaTransfer.Sign();

                    //Calculate ecdsa
                    string primaryTypeNameTransfer = "Transfer";
                    TypedData eip712TypedDataTransfer = new TypedData();
                    eip712TypedDataTransfer.Domain = new Domain()
                    {
                        Name = "Loopring Protocol",
                        Version = "3.6.0",
                        ChainId = 1,
                        VerifyingContract = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                    };
                    eip712TypedDataTransfer.PrimaryType = primaryTypeNameTransfer;
                    eip712TypedDataTransfer.Types = new Dictionary<string, MemberDescription[]>()
                    {
                        ["EIP712Domain"] = new[]
                            {
                                            new MemberDescription {Name = "name", Type = "string"},
                                            new MemberDescription {Name = "version", Type = "string"},
                                            new MemberDescription {Name = "chainId", Type = "uint256"},
                                            new MemberDescription {Name = "verifyingContract", Type = "address"},
                                        },
                        [primaryTypeNameTransfer] = new[]
                            {
                                            new MemberDescription {Name = "from", Type = "address"},            // payerAddr
                                            new MemberDescription {Name = "to", Type = "address"},              // toAddr
                                            new MemberDescription {Name = "tokenID", Type = "uint16"},          // token.tokenId 
                                            new MemberDescription {Name = "amount", Type = "uint96"},           // token.volume 
                                            new MemberDescription {Name = "feeTokenID", Type = "uint16"},       // maxFee.tokenId
                                            new MemberDescription {Name = "maxFee", Type = "uint96"},           // maxFee.volume
                                            new MemberDescription {Name = "validUntil", Type = "uint32"},       // validUntill
                                            new MemberDescription {Name = "storageID", Type = "uint32"}         // storageId
                                        },

                    };
                    eip712TypedDataTransfer.Message = new[]
                    {
                                    new MemberValue {TypeName = "address", Value = fromAddress},
                                    new MemberValue {TypeName = "address", Value = transferToAddress},
                                    new MemberValue {TypeName = "uint16", Value = req.token.tokenId},
                                    new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(req.token.volume)},
                                    new MemberValue {TypeName = "uint16", Value = req.maxFee.tokenId},
                                    new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(req.maxFee.volume)},
                                    new MemberValue {TypeName = "uint32", Value = req.validUntil},
                                    new MemberValue {TypeName = "uint32", Value = req.storageId},
                                };

                    TransferTypedData typedDataTransfer = new TransferTypedData()
                    {
                        domain = new TransferTypedData.Domain()
                        {
                            name = "Loopring Protocol",
                            version = "3.6.0",
                            chainId = 1,
                            verifyingContract = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                        },
                        message = new TransferTypedData.Message()
                        {
                            from = fromAddress,
                            to = transferToAddress,
                            tokenID = req.token.tokenId,
                            amount = req.token.volume,
                            feeTokenID = req.maxFee.tokenId,
                            maxFee = req.maxFee.volume,
                            validUntil = (int)req.validUntil,
                            storageID = req.storageId
                        },
                        primaryType = primaryTypeNameTransfer,
                        types = new TransferTypedData.Types()
                        {
                            EIP712Domain = new List<Type>()
                                        {
                                            new Type(){ name = "name", type = "string"},
                                            new Type(){ name="version", type = "string"},
                                            new Type(){ name="chainId", type = "uint256"},
                                            new Type(){ name="verifyingContract", type = "address"},
                                        },
                            Transfer = new List<Type>()
                                        {
                                            new Type(){ name = "from", type = "address"},
                                            new Type(){ name = "to", type = "address"},
                                            new Type(){ name = "tokenID", type = "uint16"},
                                            new Type(){ name = "amount", type = "uint96"},
                                            new Type(){ name = "feeTokenID", type = "uint16"},
                                            new Type(){ name = "maxFee", type = "uint96"},
                                            new Type(){ name = "validUntil", type = "uint32"},
                                            new Type(){ name = "storageID", type = "uint32"},
                                        }
                        }
                    };

                    Eip712TypedDataSigner signerTransfer = new Eip712TypedDataSigner();
                    var ethECKeyTransfer = new Nethereum.Signer.EthECKey(MMorGMEPrivateKey.Replace("0x", ""));
                    var encodedTypedDataTransfer = signerTransfer.EncodeTypedData(eip712TypedDataTransfer);
                    var ECDRSASignatureTransfer = ethECKeyTransfer.SignAndCalculateV(Sha3Keccack.Current.CalculateHash(encodedTypedDataTransfer));
                    var serializedECDRSASignatureTransfer = EthECDSASignature.CreateStringSignature(ECDRSASignatureTransfer);
                    var transferEcdsaSignature = serializedECDRSASignatureTransfer + "0" + (int)2;

                    var tokenTransferResult = await loopringService.SubmitTokenTransfer(
                        loopringApiKey,
                        exchange,
                        fromAccountId,
                        fromAddress,
                        0,
                        transferToAddress,
                        req.token.tokenId,
                        req.token.volume,
                        req.maxFee.tokenId,
                        req.maxFee.volume,
                        req.storageId,
                        req.validUntil,
                        transferEddsaSignature,
                        transferEcdsaSignature,
                        transferMemo);
                    // Console.WriteLine(tokenTransferResult);
                    validAddress.Add(transferToAddress);
                }
                Font.SetTextToBlue("Airdrop finished...");
                Utils.ShowAirdropAuditCrypto(validAddress, invalidAddress, banishAddress, transferTokenSymbol);
            }
            break;
        #endregion case 13
        #region case 14
        case "14":
            Font.SetTextToBlue(menuAndUtility.allUtilities.ElementAt(14).Value);
            Font.SetTextToBlue("Here you will pay the activation fee for the given wallet addresses.");
            Font.SetTextToYellow("A transaction has to be sent to do this. You will be sending 0.000000000000000001 LRC or ETH to each wallet.");
            var alreadyActivatedAddress = new List<string>();
            amountToTransfer = 0.000000000000000001m;
            transferMemo = "";
            transferTokenId = 3;
            transferTokenSymbol = "";
            Font.SetTextToWhite("Let's get started.");
            Font.SetTextToBlue("Do you want to send LRC(1) or ETH(0)?");
            transferTokenId = Utils.Check1Or2(transferTokenId);
            if (transferTokenId == 1)
            {
                transferTokenSymbol = "LRC";
            }
            else if (transferTokenId == 0)
            {
                transferTokenSymbol = "ETH";
            }
            howManyWallets = Utils.CheckInputDotTxt(fileName);

            var transferFeeAmountResultCheck = await loopringService.GetOffChainTransferFeeForTransferAndUpdateAccount(loopringApiKey, fromAccountId, 15); //3 is the request type for crypto transfers
            var feeAmountCheck = transferFeeAmountResultCheck.fees.Where(w => w.token == transferTokenSymbol).First().fee;

            Font.SetTextToYellow($"You will be activating {howManyWallets} wallets.");
            Font.SetTextToYellow($"and sending {amountToTransfer * howManyWallets} {transferTokenSymbol}.");
            Font.SetTextToYellow($"gas fees will be around {(decimal.Parse(feeAmountCheck) / 1000000000000000000m) * Convert.ToDecimal(howManyWallets)} {transferTokenSymbol}.");
            Font.SetTextToBlue("Memo for transfer:");
            transferMemo = Console.ReadLine()?.Trim();

            Font.SetTextToBlue("Starting activations...");
            airdropNumberOn = 0;
            using (StreamReader sr = new StreamReader($"./{fileName}"))
            {
                while ((walletAddressLine = sr.ReadLine()) != null)
                {
                    toAddressInitial = walletAddressLine;
                    var transferToAddress = walletAddressLine.ToLower().Trim();

                    Console.Write($"\r{++airdropNumberOn}/{howManyWallets}");

                    //check for ens and convert to long wallet address if so
                    if (transferToAddress.Contains(".eth"))
                    {
                        var varHexAddress = await loopringService.GetHexAddress(settings.LoopringApiKey, transferToAddress);
                        if (!String.IsNullOrEmpty(varHexAddress.data))
                        {
                            transferToAddress = varHexAddress.data;
                        }
                        else
                        {
                            invalidAddress.Add(transferToAddress);
                            Thread.Sleep(1); //for a rate limiter just incase multiple invalid ens
                            continue;
                        }
                    }

                    contains = await loopringService.CheckBanishTextFile(toAddressInitial, transferToAddress, settings.LoopringApiKey);
                    if (contains == true)
                    {
                        banishAddress.Add(toAddressInitial);
                        continue;
                    }

                    var accountInformation = await loopringService.CheckUserAccountInformationFromOwner(transferToAddress);
                    if (accountInformation != null)
                    {
                        if (accountInformation.nonce == 1 || accountInformation.tags == "FirstUpdateAccountPaid")
                        {
                            alreadyActivatedAddress.Add(toAddressInitial);
                            continue;
                        }
                    }

                    // start activation
                    var amount = (amountToTransfer * 1000000000000000000m).ToString("0");
                    var transferFeeAmountResult = await loopringService.GetOffChainTransferFeeForTransferAndUpdateAccount(loopringApiKey, fromAccountId, 15); //3 is the request type for crypto transfers
                    var feeAmount = transferFeeAmountResult.fees.Where(w => w.token == transferTokenSymbol).First().fee;
                    var transferStorageId = await loopringService.GetNextStorageId(loopringApiKey, fromAccountId, transferTokenId);

                    TransferRequest req = new TransferRequest()
                    {
                        exchange = exchange,
                        maxFee = new Token()
                        {
                            tokenId = transferTokenId,
                            volume = feeAmount
                        },
                        token = new Token()
                        {
                            tokenId = transferTokenId,
                            volume = amount
                        },
                        payeeAddr = transferToAddress,
                        payerAddr = fromAddress,
                        payeeId = 0,
                        payerId = fromAccountId,
                        storageId = transferStorageId.offchainId,
                        validUntil = Utils.GetUnixTimestamp() + (int)TimeSpan.FromDays(365).TotalSeconds,
                        tokenName = transferTokenSymbol,
                        tokenFeeName = transferTokenSymbol,
                    };

                    BigInteger[] eddsaSignatureinputs = {
                Utils.ParseHexUnsigned(req.exchange),
                (BigInteger)req.payerId,
                (BigInteger)req.payeeId,
                (BigInteger)req.token.tokenId,
                BigInteger.Parse(req.token.volume),
                (BigInteger)req.maxFee.tokenId,
                BigInteger.Parse(req.maxFee.volume),
                Utils.ParseHexUnsigned(req.payeeAddr),
                0,
                0,
                (BigInteger)req.validUntil,
                (BigInteger)req.storageId
            };

                    Poseidon poseidonTransfer = new Poseidon(13, 6, 53, "poseidon", 5, _securityTarget: 128);
                    BigInteger poseidonTransferHash = poseidonTransfer.CalculatePoseidonHash(eddsaSignatureinputs);
                    Eddsa eddsaTransfer = new Eddsa(poseidonTransferHash, loopringPrivateKey);
                    string transferEddsaSignature = eddsaTransfer.Sign();

                    //Calculate ecdsa
                    string primaryTypeNameTransfer = "Transfer";
                    TypedData eip712TypedDataTransfer = new TypedData();
                    eip712TypedDataTransfer.Domain = new Domain()
                    {
                        Name = "Loopring Protocol",
                        Version = "3.6.0",
                        ChainId = 1,
                        //ChainId = 5, //goerli
                        VerifyingContract = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                        //VerifyingContract = "0x2e76EBd1c7c0C8e7c2B875b6d505a260C525d25e",
                    };
                    eip712TypedDataTransfer.PrimaryType = primaryTypeNameTransfer;
                    eip712TypedDataTransfer.Types = new Dictionary<string, MemberDescription[]>()
                    {
                        ["EIP712Domain"] = new[]
                            {
                                            new MemberDescription {Name = "name", Type = "string"},
                                            new MemberDescription {Name = "version", Type = "string"},
                                            new MemberDescription {Name = "chainId", Type = "uint256"},
                                            new MemberDescription {Name = "verifyingContract", Type = "address"},
                                        },
                        [primaryTypeNameTransfer] = new[]
                            {
                                            new MemberDescription {Name = "from", Type = "address"},            // payerAddr
                                            new MemberDescription {Name = "to", Type = "address"},              // toAddr
                                            new MemberDescription {Name = "tokenID", Type = "uint16"},          // token.tokenId 
                                            new MemberDescription {Name = "amount", Type = "uint96"},           // token.volume 
                                            new MemberDescription {Name = "feeTokenID", Type = "uint16"},       // maxFee.tokenId
                                            new MemberDescription {Name = "maxFee", Type = "uint96"},           // maxFee.volume
                                            new MemberDescription {Name = "validUntil", Type = "uint32"},       // validUntill
                                            new MemberDescription {Name = "storageID", Type = "uint32"}         // storageId
                                        },

                    };
                    eip712TypedDataTransfer.Message = new[]
                    {
                                    new MemberValue {TypeName = "address", Value = fromAddress},
                                    new MemberValue {TypeName = "address", Value = transferToAddress},
                                    new MemberValue {TypeName = "uint16", Value = req.token.tokenId},
                                    new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(req.token.volume)},
                                    new MemberValue {TypeName = "uint16", Value = req.maxFee.tokenId},
                                    new MemberValue {TypeName = "uint96", Value = BigInteger.Parse(req.maxFee.volume)},
                                    new MemberValue {TypeName = "uint32", Value = req.validUntil},
                                    new MemberValue {TypeName = "uint32", Value = req.storageId},
                                };

                    TransferTypedData typedDataTransfer = new TransferTypedData()
                    {
                        domain = new TransferTypedData.Domain()
                        {
                            name = "Loopring Protocol",
                            version = "3.6.0",
                            chainId = 1,
                            //chainId = 5, //goerli
                            verifyingContract = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                            //verifyingContract = "0x2e76EBd1c7c0C8e7c2B875b6d505a260C525d25e",
                        },
                        message = new TransferTypedData.Message()
                        {
                            from = fromAddress,
                            to = transferToAddress,
                            tokenID = req.token.tokenId,
                            amount = req.token.volume,
                            feeTokenID = req.maxFee.tokenId,
                            maxFee = req.maxFee.volume,
                            validUntil = (int)req.validUntil,
                            storageID = req.storageId
                        },
                        primaryType = primaryTypeNameTransfer,
                        types = new TransferTypedData.Types()
                        {
                            EIP712Domain = new List<Type>()
                                        {
                                            new Type(){ name = "name", type = "string"},
                                            new Type(){ name="version", type = "string"},
                                            new Type(){ name="chainId", type = "uint256"},
                                            new Type(){ name="verifyingContract", type = "address"},
                                        },
                            Transfer = new List<Type>()
                                        {
                                            new Type(){ name = "from", type = "address"},
                                            new Type(){ name = "to", type = "address"},
                                            new Type(){ name = "tokenID", type = "uint16"},
                                            new Type(){ name = "amount", type = "uint96"},
                                            new Type(){ name = "feeTokenID", type = "uint16"},
                                            new Type(){ name = "maxFee", type = "uint96"},
                                            new Type(){ name = "validUntil", type = "uint32"},
                                            new Type(){ name = "storageID", type = "uint32"},
                                        }
                        }
                    };

                    Eip712TypedDataSigner signerTransfer = new Eip712TypedDataSigner();
                    var ethECKeyTransfer = new Nethereum.Signer.EthECKey(MMorGMEPrivateKey.Replace("0x", ""));
                    var encodedTypedDataTransfer = signerTransfer.EncodeTypedData(eip712TypedDataTransfer);
                    var ECDRSASignatureTransfer = ethECKeyTransfer.SignAndCalculateV(Sha3Keccack.Current.CalculateHash(encodedTypedDataTransfer));
                    var serializedECDRSASignatureTransfer = EthECDSASignature.CreateStringSignature(ECDRSASignatureTransfer);
                    var transferEcdsaSignature = serializedECDRSASignatureTransfer + "0" + (int)2;

                    var tokenTransferResult = await loopringService.SubmitPayPayeeUpdateAccountFee(
                        loopringApiKey,
                        exchange,
                        fromAccountId,
                        fromAddress,
                        0,
                        transferToAddress,
                        req.token.tokenId,
                        req.token.volume,
                        req.maxFee.tokenId,
                        req.maxFee.volume,
                        req.storageId,
                        req.validUntil,
                        transferEddsaSignature,
                        transferEcdsaSignature,
                        transferMemo,
                        "true");
                    // Console.WriteLine(tokenTransferResult);
                    validAddress.Add(toAddressInitial);
                }

            }
            Console.WriteLine();
            Font.SetTextToBlue("Activations finished...");
            Utils.ShowAirdropAuditActivation(validAddress, invalidAddress, banishAddress, transferTokenSymbol);
            if (alreadyActivatedAddress.Count() > 0)
            {
                Font.SetTextToGreen("The below addresses were already activated.");
                foreach (var item in alreadyActivatedAddress)
                {
                    Console.WriteLine(item);
                }
            }
            break;
        #endregion case 14
        #region case 15
        case "15":
            Font.SetTextToBlue(menuAndUtility.allUtilities.ElementAt(14).Value);
            Font.SetTextToBlue("Here you will get all the wallet addresses and Analytics from the LoopPhunks collection.");
            Font.SetTextToYellow("Note that you must own a LoopPhunks to use this.");

            signedMessage = EDDSAHelper.EddsaSignUrl(
            loopringPrivateKey,
            HttpMethod.Get,
            new List<(string Key, string Value)>() { ("accountId", fromAccountId.ToString()) },
            null,
            Constants.ApiKeyUrl,
            "https://api3.loopring.io/");
            apiKey = await loopringService.GetApiKey(fromAccountId, signedMessage);

            if (apiKey == loopringApiKey)
            {
                Font.SetTextToWhite("Collecting data, this may take a few minutes...");

                //var loopPhunksInformation = LoopPhunks.GetLoopPhunksInformation();
                //var LoopExchangeData = await loopExchangeService.GetLoopPhunksData();
                //var nftInformationAndOwner = await loopringService.GetNftHoldersLoopPhunks(loopringApiKey, loopPhunksInformation);

                //ExcelFile.CreateExcelFile(LoopExchangeData, nftInformationAndOwner);
                //Console.WriteLine($@"Finished. Your file can be found {AppDomain.CurrentDomain.BaseDirectory}\LoopPhunksDataCOOL.xlsx");

                var nftInformationAndOwner = await loopringService.GetUserNftTransfer(193354, "0x15f2d8891fe0b5a6836de3a08d9d21e813978e93fb000e446c1957fb6c3474e4");

                foreach (var nft in nftInformationAndOwner)
                {
                    Console.WriteLine(nft.payeeAddress);
                    using StreamWriter file = new("./test.txt", append: true);
                    file.WriteLine($"{nft.payeeAddress}");
                }


            }
            else
            {
                Font.SetTextToRed("It looks like the appsetting file is not setup correctly because the accountId does not match the Loopring Api Key.");
            }
            break;
        #endregion case 15
        #region case 16
        case "16":
            Font.SetTextToBlue(menuAndUtility.allUtilities.ElementAt(15).Value);
            Font.SetTextToBlue("Here you will get all the current holders of an IMX collection.");
            Font.SetTextToYellow("Note that you must own a LoopPhunks to use this.");

            signedMessage = EDDSAHelper.EddsaSignUrl(
            loopringPrivateKey,
            HttpMethod.Get,
            new List<(string Key, string Value)>() { ("accountId", fromAccountId.ToString()) },
            null,
            Constants.ApiKeyUrl,
            "https://api3.loopring.io/");
            apiKey = await loopringService.GetApiKey(fromAccountId, signedMessage);

            if (apiKey == loopringApiKey)
            {

                Font.SetTextToBlue("What is the Collection Address?");
                string collectionResponse = Utils.ReadLineWarningNoNulls("What is the Collection Address?");

                Font.SetTextToWhite("Collecting data, this may take a few minutes...");

                var minterInformation = new List<AssetExcel>();

                var collectionMints = await imxService.GetCollectionAssets(collectionResponse);

                foreach (var minter in collectionMints)
                {
                    minterInformation.Add(new AssetExcel
                    {
                        name = minter.name,
                        user = minter.user
                    });
                }
                using (var writer = new StreamWriter("./CollectionHolders.csv"))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(minterInformation.OrderBy(x => x.name));
                }
                Console.WriteLine($@"Finished. Your file can be found {AppDomain.CurrentDomain.BaseDirectory}\CollectionHolders.xlsx");
            }
            else
            {
                Font.SetTextToRed("It looks like the appsetting file is not setup correctly because the accountId does not match the Loopring Api Key.");
            }
    break;
        #endregion case 16
    }
    userResponseReadyToMoveOn = Menu.EndOfLoopDropSharpFunctionality(validAddress, invalidAddress, banishAddress, userMintsAndTotalList,
        nftHoldersAndTotalList);
}
Menu.FooterForLoopDropSharp();
Console.ReadLine();