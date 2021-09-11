namespace SST.Gameplay
{
    public class Grid2D<TValue> {
        public int Size => xSize * ySize;
        public int Length => grid.Length;

        public int xSize;
        public int ySize;

        public TValue[] grid;
        public TValue emptyCell;


        public TValue this[int index] {
            get => Get(index);
            set => Set(index, value);
        }

        public TValue this[int x, int y] {
            get => Get(x, y);
            set => Set(x, y, value);
        }

        public Grid2D(int xSize, int ySize, TValue emptyCell) {
            this.xSize = xSize;
            this.ySize = ySize;
            grid = new TValue[Size];
            this.emptyCell = emptyCell;
            Fill(emptyCell);
        }

        public void Fill(TValue value) {
            for (int i = 0; i < Size; i++) {
                grid[i] = value;
            }
        }

        public void Set(int index, TValue value) {
            grid[index] = value;
        }

        public void Set(int x, int y, TValue value) {
            Set(Index(x, y), value);
        }

        public bool SetSafe(int index, TValue value) {
            if (index > 0 && index < (xSize * ySize)) {
                Set(index, value);
                return true;
            }

            return false;
        }

        public bool SetSafe(int x, int y, TValue value) {
            return SetSafe(Index(x, y), value);
        }

        public TValue Get(int index) {
            return grid[index];
        }

        public TValue Get(int x, int y) {
            return Get(Index(x, y));
        }

        public bool GetSafe(int index, out TValue value) {
            if (index > 0 && index < (xSize * ySize)) {
                value = Get(index);
                return true;
            }

            value = default;
            return false;
        }

        public bool GetSafe(int x, int y, out TValue value) {
            return GetSafe(Index(x, y), out value);
        }

        public int Index(int x, int y) {
            return x + xSize * y;
        }

        public void Position(int index, out int x, out int y) {
            x = index % xSize;
            y = index / ySize;
        }

        public UnityEngine.Vector2Int Position(int index) {
            int x, y;
            Position(index, out x, out y);
            return new UnityEngine.Vector2Int(x, y);
        }
        public TValue[] GetNeighbors(int index) {
            TValue[] result = new TValue[4];
            GetNeighborsNoAlloc(index, result);
            return result;
        }

        public void GetNeighborsNoAlloc(int index, TValue[] result) {
            int x, y;
            Position(index, out x, out y);

            // North
            if (y >= (ySize - 1)) result[0] = emptyCell;
            else result[0] = grid[Index(x, y + 1)];

            // East
            if (x >= (xSize - 1)) result[1] = emptyCell;
            else result[1] = grid[Index(x + 1, y)];

            // South
            if (y <= 0) result[2] = emptyCell;
            else result[2] = grid[Index(x, y - 1)];

            // West
            if (x <= 0) result[3] = emptyCell;
            else result[3] = grid[Index(x - 1, y)];
        }
    }
}