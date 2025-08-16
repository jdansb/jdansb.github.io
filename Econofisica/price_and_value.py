import numpy  as np
import random
from scipy.stats import pearsonr


## SETTING THE INITIAL CONDITION

# Parameters
N = 100         # Number of workers
L = 3           # Number of commodities
M = 100*N       # Total money in the simulation
R = 20          # Maximum possible consumption time
C = 2           # Sector switching factor
p = 10000        # Simulation duration
G = 1000        # Period for storing the data

# % R1
# For calculations
Error = np.full(N,0.0)                  # Error of each agent in the previous year
E     = np.zeros((N,L))                 # Commodities in possession of each agent
A     = np.random.randint(0,L,N)        # Vector with the commodity currently produced by the agent
D     = np.zeros((N,L))                 # Matrix with the consumption deficit of each agent for each commodity
m     = np.full(N, int(M/N))            # Money of each agent

#Production of vectors l and c that respect eta=1
while(True):
    l=np.random.random(L)                        # Production rate
    c=np.random.random(L)                        # Deficit rate
    F=np.sum(c/l)                               
    c=c/F                                       
    T=round(C*max(1/c))                          # Calculating the period for sector switching
    if(max(1/c)<=R):                             # If the maximum consumption time is acceptable
        break  

Consumption  = 1/c                               # Time needed to consume
Production = 1/l                                 # Time needed to produce


trades = [[] for x in range (L)]                 # Register the trades for each commodity
# %% SIMULATION

for step in range(p):
    # P1
    for i in range(N):  # Loop over agents
      j = A[i]          # Commodity the agent is producing
      E[i][j]+=l[j]     # Increase production

    # C1
    for i in range(N):     # For each agent i
      for j in range(L):   # And commodity j
        D[i][j] +=c[j]     # Increase deficit
        # Calculate the smallest value
        consumption  = int(D[i][j]) if (int(D[i][j]) < int(E[i][j])) else int(E[i][j])
        D[i][j] -= consumption  # Reduce from deficit
        E[i][j] -= consumption  # Reduce from goods


    # M1
    Nres=[mer for mer in range(L)]                  # Commodities still unresolved

    while(len(Nres)>0):                             # Resolve all commodities
        j = random.choice(Nres)                     # Select a random commodity
        sellers =[];buyers =[]                      # Create list of possible sellers and buyers
        for i in range(N):
            if(int(E[i][j])>int(D[i][j])):          # If they have more than they want
                sellers.append(i)                           
            if(int(D[i][j])>int(E[i][j])):          # If they want more than they have
                buyers.append(i)
        if (len(buyers)==0 or len(sellers)==0):     # If no more buyers or sellers
           Nres.remove(j)                           # Remove commodity from list
        else:                                       # Otherwise, pick a random buyer and seller and call E1           
            buy=random.choice(buyers)
            sel=random.choice(sellers)
            pbuy=random.randint(0,m[buy])           # Price evaluated by buyer
            psel=random.randint(0,m[sel])           # Price evaluated by seller
            a=min(pbuy,psel)                        # Minimum price
            b=max(pbuy,psel)                        # Maximum price
            price= random.randint(a,b)
            if(m[buy]>=price):                      # If the buyer has enough money
                m[buy]-=price                       # Buyer loses money
                m[sel]+=price                       # Seller gains money
                E[buy][j]+=1                        # Buyer gains item
                E[sel][j]-=1                        # Seller loses item
                trades[j].append(price)             # We record the transaction


    #  S1
    if(step%T==0 ):                         # If it is the right moment, then switch sectors
        for i in range(N):                  # Loop through all agents
            e = np.linalg.norm(D[i])
            if(e>Error[i]):                 # If the error is greater, switch sector
                 A[i]=random.randint(0,L-1) # Randomly
            Error[i]=e                      # Update the previous error    

    # % Check the prices and correlation
    if((step+1)%G==0):
        means=[]
        for x in range(L):
          means.append(np.array(trades[x]).mean())   # Calculate the average price of each commodity
        correlation, _ = pearsonr(means, Production) # Test correlation with values
        print("The Pearson correlation between the average price and the value of each commodity is {:.2f}".format(correlation))

        for k in range(L):
          print('The average price of commodity {:.0f} is R${:.2f}, and its value {:.2f}'.format(k,means[k],Production[k]))

        a, b = np.polyfit(Production, means, 1)
        print(f"The relationship between price and value can be given by the line equation:  = {a:.2f}v + {b:.2f} with a ratio of {b/a:.2f}")
        backup=trades.copy()
        trades =[[] for x in range (L)]
        print('---')

