using System;

namespace Mafia
{
    public class Stage
    {
        private string fileName;
        private string title;
        private int numRows;
        private int numCols;
        private string[] source;

        public Stage(string fileName, string title, int numRows, int numCols, string[] source)
        {
            this.fileName = fileName;
            this.title = title;
            this.numRows = numRows;
            this.numCols = numCols;
            this.source = source;
        }

        public string Title
        {
            get
            {
                return title;
            }
        }

        public GameScene CreateGame()
        {
            GameScene game = new GameScene(title, numRows, numCols);
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    try
                    {
                        switch (source[row][col])
                        {
                            case ' ':
                                game.Map[row, col] = Map.NONE;
                                break;
                            case '<':
                                if (row != numRows - 1 && source[row + 1][col] == 'F') game.AddFallBlock(row, col, Map.SPIKE_LEFT);
                                else if (row != numRows - 1 && source[row + 1][col] == 'E') game.AddDelayedFallBlock(row, col, Map.SPIKE_LEFT);
                                else game.Map[row, col] = Map.SPIKE_LEFT;
                                break;
                            case 'A':
                                game.Map[row, col] = Map.SPIKE_UP;
                                break;
                            case '>':
                                if (row != numRows - 1 && source[row + 1][col] == 'F') game.AddFallBlock(row, col, Map.SPIKE_RIGHT);
                                else if (row != numRows - 1 && source[row + 1][col] == 'E') game.AddDelayedFallBlock(row, col, Map.SPIKE_RIGHT);
                                else game.Map[row, col] = Map.SPIKE_RIGHT;
                                break;
                            case 'V':
                                if (row != numRows - 1 && source[row + 1][col] == 'F') game.AddFallBlock(row, col, Map.SPIKE_DOWN);
                                else if (row != numRows - 1 && source[row + 1][col] == 'E') game.AddDelayedFallBlock(row, col, Map.SPIKE_DOWN);
                                else game.Map[row, col] = Map.SPIKE_DOWN;
                                break;
                            case 'P':
                                game.AddPlayer(row, col, int.Parse(source[row + 1][col].ToString()));
                                break;
                            case 'C':
                                game.AddCoin(row, col);
                                break;
                            case 'L':
                                game.AddLift(row, col, int.Parse(source[row][col + 1].ToString()));
                                break;
                            case 'R':
                                game.Map[row, col] = Map.LIFT_RETURN;
                                break;
                            case 'H':
                                game.AddSpring(row, col);
                                break;
                            case 'B':
                                game.AddBox(row, col);
                                break;
                            case 'S':
                                game.AddSwitch(row, col);
                                break;
                            case 'D':
                                game.AddDoor(row, col, int.Parse(source[row + 1][col].ToString()));
                                if (source[row + 1][col - 1] == '<' || source[row + 1][col + 1] == '>')
                                {
                                    game.Map[row + 1, col] = Map.DOOR_SLIDE;
                                }
                                game.Map[row - 1, col] = Map.DOOR_SLIDE;
                                game.Map[row - 2, col] = Map.DOOR_SLIDE;
                                game.Map[row - 3, col] = Map.DOOR_SLIDE;
                                break;
                            default:
                                if ('a' <= source[row][col] && source[row][col] <= 'z')
                                {
                                    if (row != numRows - 1 && source[row + 1][col] == 'F') game.AddFallBlock(row, col, 1);
                                    else if (row != numRows - 1 && source[row + 1][col] == 'E') game.AddDelayedFallBlock(row, col, 1);
                                    else game.Map[row, col] = 1;
                                }
                                break;
                        }
                    }
                    catch
                    {
                        throw new Exception(fileName + " ‚Ì " + (row + 4) + " s " + (col + 1) + " —ñ‚Ì•Ó‚è‚ª‚¨‚©‚µ‚¢‚Å‚·„ƒ");
                    }
                }
            }
            return game;
        }
    }
}
