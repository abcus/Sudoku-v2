using System.Collections.Generic;

namespace Sudoku_v2
{
    // Constants
    class Constants
    {
        public static string ALL = "123456789";
        public static int ROW = 0, COLUMN = 1, SQUARE = 2, FIRST_PEER_UNIT = 0, LAST_PEER_UNIT = 7, FIRST_PEER_COMBINED = 0, LAST_PEER_COMBINED = 19;
        public static Dictionary<Coord, Coord[,]> peerListDictionaryUnit = new Dictionary<Coord, Coord[,]>();
        public static Dictionary<Coord, Coord[]> peerListDictionaryCombined = new Dictionary<Coord, Coord[]>();

        // Returns array of peers
        public static void populatePeerDictionary() {

            for (int i = 0; i < 9; i++) {
                for (int j = 0; j < 9; j++) {
                    Coord cell = new Coord(i, j);
                    Coord[,] peerListUnit = new Coord[3,8];
                    int peerUnitIndex = 0;
                    Coord[] peerListCombined = new Coord[20];
                    int peerCombinedIndex = 0;

                    // Peers in same row
                    for (int k = 0; k < 9; k++) {
                        if (k != j) {
                            peerListUnit[Constants.ROW, peerUnitIndex++] = new Coord(i, k);
                            peerListCombined[peerCombinedIndex++] = new Coord(i, k);
                        }
                    }
                    // Peers in same column
                    peerUnitIndex = 0;
                    for (int l = 0; l < 9; l++) {
                        if (l != i) {
                            peerListUnit[Constants.COLUMN, peerUnitIndex++] = new Coord(l, j);
                            peerListCombined[peerCombinedIndex++] = new Coord(l, j);
                        }
                    }
                    // Peers in same square
                    peerUnitIndex = 0;
                    for (int m = 0; m < 3; m++) {
                        for (int n = 0; n < 3; n++) {
                            if (i / 3 * 3 + m != i || j / 3 * 3 + n != j) {
                                peerListUnit[Constants.SQUARE, peerUnitIndex++] = new Coord(i / 3 * 3 + m, j / 3 * 3 + n);
                            }
                            if (i / 3 * 3 + m != i && j / 3 * 3 + n != j) {
                                peerListCombined[peerCombinedIndex++] = new Coord(i / 3 * 3 + m, j / 3 * 3 + n);
                            }
                        }
                    }
                    peerListDictionaryUnit.Add(cell, peerListUnit);
                    peerListDictionaryCombined.Add(cell, peerListCombined);
                }
            }
        }
    }
}
