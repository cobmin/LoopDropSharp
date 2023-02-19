New application: https://maizehelps.art/

![](https://github.com/cobmin/LoopDropSharp/blob/Development/GitHubBanner.png)
### Find Nft holders, airdrop Nfts, mass transfer cryto, and more on Loopring. Quickly find what you need and send. Save time and money with LoopDropSharp.    

## Current Functionality:

![image](https://user-images.githubusercontent.com/97369738/205186643-dab2206e-76d4-42f4-96e9-42f547f4e93a.png)

## Setup

Download one of the compiled releases in the [Releases](https://github.com/cobmin/LoopDropSharp/releases) section and unzip it into a location of your choice. You will need to edit the included appsettings.json file with your own Loopring details. 

Here is a video going over the setup: https://youtu.be/P0EvuxfpCR4

This application uses a ***MetaMask/GameStop*** private key to sign the transfers. You will need to export that out from your metamask/gamestop Wallet. You can export the Loopring related account details from the "Security" tab while logged into https://loopring.io. Make sure these details are from the same Wallet. *Note* that you cannot perform transfers with a Loopring wallet

macOS users: You also need to run the following command in the unzipped folder of LoopDropSharp to turn it into an executable in order to run it. You may also need to add it as a trusted application if it get's blocked from running (Settings > Security & Privacy).

```
chmod +x LoopDropSharp
If compiling yourself please read the section about it below.
```
DO NOT SHARE ANY API OR PRIVATE KEYS with anyone else!!!!!!! 

You will need to change the "appsettings.json" file in the project directory with the necessary information. The video above covers this. 

```json
{
  "Settings": {
    "LoopringApiKey": "ksdBlahblah", //Your loopring api key.  DO NOT SHARE THIS AT ALL.
    "LoopringPrivateKey": "0xblahblah", //Your loopring private key.  DO NOT SHARE THIS AT ALL.
    "MMorGMEPrivateKey": "asadasdBLahBlah", //Private key from metamask. DO NOT SHARE THIS AT ALL.
    "LoopringAddress": "0xblahabla", //Your loopring address
    "LoopringAccountId": 40940, //Your loopring account id
    "ValidUntil": 1700000000, //How long this transfer should be valid for. Shouldn't have to change this value
    "MaxFeeTokenId": 1, //The token id for the fee. 0 for ETH, 1 for LRC
    "Exchange": "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4" //Loopring Exchange address
  }
}
```
After setting up the appsettings.json, launch LoopDropSharp and get started.

## Input.txt setup

### 2. Find Nft Datas from Nft Ids.
In the Input.txt located in the project directory add Nft Ids. You will have one Nft Id per line.

### 5. Find Nft Holders from Nft Data.
In the Input.txt located in the project directory add an Nft Data. You will have one Nft Data per line.

### 7. Find Nft Holders who own all given Nft Data
In the Input.txt located in the project directory add an Nft Data. You will have one Nft Data per line.

### 8. Airdrop the same NFT to any users.
In the Input.txt located in the project directory add a wallet addresses. You will have one wallet address per line. Each wallet address will be one transfer. Be sure to have enough LRC/ETH for each transfer. You can add a long wallet address or the ENS.

### 9. Airdrop the same NFT to any users with different amounts.
In the Input.txt located in the project directory add a wallet address a comma and then the amount you want to send (example: 0x4a71e0267207cec67c78df8857d81c508d43b00d,2). You will have one wallet address and one amount per line. Each wallet address will be one transfer. Be sure to have enough LRC/ETH for each transfer. You can add a long wallet address or the ENS.

### 10. Airdrop unique NFTs to any users
In the Input.txt located in the project directory add a wallet address a comma and then the nft data (example: 0x4a71e0267207cec67c78df8857d81c508d43b00d,0x103cb20d3b310873f711d25758d57f18ba77a6b7842ae605d662e0ddd908ed5a). You will have one wallet address and nft data per line. Each wallet address will be one transfer. Be sure to have enough LRC/ETH for each transfer. You can add a long wallet address or the ENS.

### 12. Airdrop LRC/ETH to any users.
In the Input.txt located in the project directory add a wallet addresses. You will have one wallet address per line. Each wallet address will be one transfer of LRC/ETH. Be sure to have enough LRC/ETH for each transfer. You can add a long wallet address or the ENS.

### 13. Airdrop LRC/ETH to any users with different amounts
In the Input.txt located in the project directory add a wallet address a comma and then the amount of LRC/ETH to send (example: 0x4a71e0267207cec67c78df8857d81c508d43b00d,50.25). You will have one wallet address and one amount per line. Each wallet address will be one transfer. Be sure to have enough LRC/ETH for each transfer. You can add a long wallet address or the ENS.

### 14. Pay Loopring activation fee for wallets
In the Input.txt located in the project directory add a wallet addresses. You will have one wallet address per line. Each wallet address will be one transfer of LRC/ETH. Be sure to have enough LRC/ETH for each transfer. You can add a long wallet address or the ENS.

## Banish.txt setup
In the Banish.txt located in the project directory add wallet addresses that you do not want to send to. If you have a wallet address that you never want to airdrop an Nft to or send crypto to then you can place them on this list. The application checks this list before sending Nfts or crypto. You can add a long wallet address or the ENS.

### 11. Send all Nfts owned by banished addresses to the dead address
In the Banish.txt located in the project directory add a wallet address whose Nfts you want to remove from your wallet. You can add a long wallet address or the ENS.

LoopDropSharp Nft collection: https://loopexchange.art/collection/loopdropsharp
More information: https://cobmin.io/posts/LoopDropSharp

### Supporting Me
If you would like to support me you can purchase one of my Nfts here, https://loopexchange.art/collection/loopdropsharp or donate to my Loopring wallet, jacobhuber.eth.

<img src="https://user-images.githubusercontent.com/97369738/189735788-5ca5ff22-3e28-4c2d-9185-b1121d78a6e2.jpeg" width="150" />

## Credits
The original boiler plate was originally made by Fudgey, then I did some modifications. It was then time for LoopDropSharp to be created.

Also thanks to:
ItsMonty.eth for the original NFT Transfer code in the [LooPyMinty](https://github.com/Montspy/LooPyMinty) repo which I converted to C#.
Also, thanks to Taranasus for his [LoopringSharp](https://github.com/taranasus/LoopringSharp) repo for the ECDSA signing which I also used.
Fudgey's PoseidonSharp library was used for generating the Poseidon hashes and EDDSA signing.
