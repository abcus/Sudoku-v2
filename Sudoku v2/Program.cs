using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Sudoku_v2 {
	class Program {

	    static int nodes = 0;
        static Boolean[,] initial = new Boolean[9, 9];

        static void Main(string[] args) {
            Constants.populatePeerDictionary();
		    String[,] position = new String[9,9];
            initialize_position(position);
            string_to_position(position);
            printBoard(solve(position));
            Console.WriteLine(nodes);
		}

        static void initialize_position(String[,] position) {
            for (int row = 0; row < 9; row++) {
                for (int column = 0; column < 9; column++) {
                    position[row, column] = Constants.ALL;
                }
            }
        }
        
        // Makes a move "number_assign" at the location "coordinate"
        static string[,] make(String[,] inputBoard, Coord coordinate, int number_assign) {

            // Goes through all of the possible values that are not the "number"
            for (int i = 1; i <= 9; i++) {
                if (i != number_assign) {
                    // Eliminates that possibility from the location "coordinate"
                    inputBoard = eliminate(inputBoard, coordinate, i);
                    if (inputBoard == null) {
                        return null;
                    }
                }
            }
            return inputBoard;
        }

        static string[,] eliminate(string[,] inputBoard, Coord coordinate, int number_elim) {

            // Check if the possibility has already been climinated
            if (inputBoard[coordinate.row, coordinate.column].Contains(number_elim.ToString()) == false) {
                return inputBoard;
            }
            // Eliminate the possibility from the cell
            inputBoard[coordinate.row, coordinate.column] = inputBoard[coordinate.row, coordinate.column].Replace(number_elim.ToString(), "");

            // Check to see if the last possibility has been eliminated, if so then contradiction
            if (inputBoard[coordinate.row, coordinate.column].Length == 0) {
                return null;
            }

            Coord[,] peerListUnit;
            Constants.peerListDictionaryUnit.TryGetValue(coordinate, out peerListUnit);

            // If all possibilities except 1 have been eliminated, then eliminate that value from each of the cell's peers
            if (inputBoard[coordinate.row, coordinate.column].Length == 1) {
                char final_value = inputBoard[coordinate.row, coordinate.column][0];

                // Loops through each of the peers
                for (int i = Constants.ROW; i <= Constants.SQUARE; i++) {
                    for (int j = Constants.FIRST_PEER_UNIT; j <= Constants.LAST_PEER_UNIT; j++) {
                        inputBoard = eliminate(inputBoard, peerListUnit[i, j], final_value - 48);
                        if (inputBoard == null) {
                            return null;
                        }
                    }
                }
            }

            // If the frequency of a value is 1 and the square with that value has not been assigned, then assign that value to the square
            for (int i = Constants.ROW; i <= Constants.SQUARE; i++) {

                // Goes through every possible value, and for each value goes through the square that had the number eliminated from it and its peers and finds the frequency of all numbers
                for (int j = 1; j <= 9; j++) {
                    List<Coord> value_locations = new List<Coord>();
                    
                    if (inputBoard[coordinate.row, coordinate.column].Contains(j.ToString())) {
                        value_locations.Add(coordinate);
                    }
                    for (int k = Constants.FIRST_PEER_UNIT; k <= Constants.LAST_PEER_UNIT; k++) {
                        if (inputBoard[peerListUnit[i, k].row, peerListUnit[i, k].column].Contains(j.ToString())) {
                            value_locations.Add(new Coord(peerListUnit[i, k].row, peerListUnit[i, k].column));
                        } 
                    }
                    // If a unit cannot have a certain value, then contradiction
                    if (value_locations.Count == 0) {
                        return null;
                    }
                    // If a certain value is only a possibility in one cell in the unit, then assign that value to the cell
                    if (value_locations.Count == 1) {
                        inputBoard = make(inputBoard, value_locations[0], j);
                        if (inputBoard == null) {
                            return null;
                        }
                    }
                }
            }
            return inputBoard;
        }

        // Returns the cell with the fewest number of possible values
        static Coord cell_selector(String[,] inputBoard) {
            int row = -1;
            int column = -1;
            int min_possibilities = 10;

            for (int i = 0; i < 9; i++) {
                for (int j = 0; j < 9; j++) {
                    if (inputBoard[i, j].Length < min_possibilities && inputBoard[i, j].Length > 1) {
                        row = i;
                        column = j;
                        min_possibilities = inputBoard[i, j].Length;
                    }
                }
            }
            Coord coordinate = new Coord(row, column);
            return coordinate;
        }

	    static int value_selector(String[,] inputBoard, Coord coordinate) {
            Coord[] peerListCombined;
            Constants.peerListDictionaryCombined.TryGetValue(coordinate, out peerListCombined);


	        for (int i = 0; i < inputBoard[coordinate.row, coordinate.column].Length; i++) {
	            // Add this in later, select values based on least number of conflicts or something (copy from the one online)
	        }

            return inputBoard[coordinate.row, coordinate.column][0] - 48;
	    }

        static bool isSolved(String[,] inputBoard) {
            for (int i = 0; i < 9; i++) {
                for (int j = 0; j < 9; j++) {
                    if (inputBoard[i, j].Length != 1) {
                        return false;
                    }
                }
            }
            return true;
        }

        static String[,] solve(String[,] inputBoard) {

            String[,] backup;

            if (inputBoard == null) {
                return null;
            }
            if (isSolved(inputBoard)) {
                return inputBoard;
            }

            // Select which cell to search
            Coord coordinate = cell_selector(inputBoard);

            while (inputBoard[coordinate.row, coordinate.column].Length > 0) {
                
                // Select which value in the selected cell to try
                int value = value_selector(inputBoard, coordinate);
                backup = solve(make(array_copy(inputBoard), coordinate, value));
                if (backup != null) {
                    return backup;
                }
                nodes++;
                inputBoard = eliminate(inputBoard, coordinate, value);
                if (inputBoard == null) {
                    return null;
                }
            }
            return null;
        }

        static void string_to_position(String[,] position) {
            Console.WriteLine("Enter 81-character sudoku string:");
            String boardString = Console.ReadLine();
            int char_index = 0;
            for (int row = 0; row < 9; row++) {
                for (int column = 0; column < 9; column++) {
                    if (boardString[char_index] != '0') {
                        Coord cell = new Coord(row, column);
                        make(position, cell, boardString[char_index] - '0');
                        initial[row, column] = true;
                    }
                    char_index++;
                }
            }
        }

        static void position_to_string(String[,] position) {
            string numbers = "";
            for (int i = 0; i < 9; i++) {
                for (int j = 0; j < 9; j++) {
                    if (position[i, j].Length == 1) {
                        numbers += position[i, j];
                    } else {
                        numbers += "0";
                    }
                }
            }
            Console.WriteLine(numbers);
        }

        static string[,] array_copy(string[,] array) {
            String[,] array_clone = new String[9, 9];
            for (int row = 0; row < 9; row++) {
                for (int column = 0; column < 9; column++) {
                    array_clone[row, column] = String.Copy(array[row, column]);
                }
            }
            return array_clone;
        }

        static void printBoard(string[,] inputBoard) {
            for (int i = 0; i < 10; i++) {
                Console.WriteLine(i != 0 ? i < 9 ? i % 3 == 0 ? "╠═══╪═══╪═══╬═══╪═══╪═══╬═══╪═══╪═══╣" : "╟───┼───┼───╫───┼───┼───╫───┼───┼───╢" : "╚═══╧═══╧═══╩═══╧═══╧═══╩═══╧═══╧═══╝" : "╔═══╤═══╤═══╦═══╤═══╤═══╦═══╤═══╤═══╗");
                for (int j = 0; j < 10; j++) {
                    if (i == 9) {
                        Console.Write("");
                    } else if (j == 0) {
                        Console.Write("║ ");
                    } else {
                        if (inputBoard[i, (j - 1)].Length == 1 && initial[i, (j-1)] == true) {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(inputBoard[i, (j - 1)]);
                            Console.ForegroundColor = ConsoleColor.White;
                        } else if (inputBoard[i, (j-1)].Length == 1) {
                            Console.Write(inputBoard[i, (j - 1)]);
                        } else {
                            Console.Write(" ");
                        }
                        Console.Write((j % 3 == 0 ? " ║ " : " │ ") + (j == 9 ? "\n" : ""));
                    }
                }
            }
        }

        static void printOptions(string[,] inputBoard) {
            Console.WriteLine("       0           1           2           3           4           5           6           7           8");
            for (int i = 0; i < 10; i++) {
                Console.WriteLine(i != 0 ? i < 9 ? i % 3 == 0 ? " ╠═══════════╪═══════════╪═══════════╬═══════════╪═══════════╪═══════════╬═══════════╪═══════════╪═══════════╣" : " ╟───────────┼───────────┼───────────╫───────────┼───────────┼───────────╫───────────┼───────────┼───────────╢" : " ╚═══════════╧═══════════╧═══════════╩═══════════╧═══════════╧═══════════╩═══════════╧═══════════╧═══════════╝" : " ╔═══════════╤═══════════╤═══════════╦═══════════╤═══════════╤═══════════╦═══════════╤═══════════╤═══════════╗");
                Console.Write(i < 9 ? Convert.ToString(i) : "");
                for (int j = 0; j < 10; j++) {
                    Console.Write(i == 9 ? "" : j == 0 ? "║ " : (inputBoard[i, (j - 1)].Length < 9 ? (inputBoard[i, (j - 1)]).PadRight(9) : (inputBoard[i, (j - 1)])) + (j % 3 == 0 ? " ║ " : " │ ") + (j == 9 ? "\n" : ""));
                }
            }
        }
	}
    
    struct Coord {
        internal int row, column;

        public Coord(int row, int column) {
            this.row = row;
            this.column = column;
        }
        public override string ToString() {
            return "(" + row + ", " + column + ")";
        }
    }
}
