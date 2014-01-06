The mathematics behind Battleship are pretty simple. 

In this implementation I am assuming the most popular variant of a 10x10 grid with the following ships placed in the board: 

Type of ship 				Size
aircraft carrier 			5
battleship 					4
submarine 					3
destroyer (or cruiser) 		3
patrol boat (or destroyer) 	2

An alternative placing is:

Type of ship 				Size
1x aircraft carrier 			4
2x battleship 					3
3x patrol boat (or destroyer) 	2
4x submarine 					1

http://en.wikipedia.org/wiki/Battleship_%28game%29

Overall there are 17 squares to be hit out of a 100 in the first variant and 20 in the second variant, making the second variant slightly easier. 

Mathematically, we're looking at a combination of 17 out of 100, or a combination of 20 out of 100 respectively. 

C(100, 17) = 100! / (17! * 83!) = 6.65013487e+18
c(100, 20) = 100! / (20! * 80!) = 5.35983370e+20

Naturally, only a subset of those combinations are valid boards, because ships cannot touch, and they must be ships, that is, there 
cannot be 17 or 20 single squares spread across the board.

Ideally, the ships would also be spread evenly across the board, In my implementation I consider a distribution good if there are at least 
two squares to be hit in each quadrant of the board. 