namespace SST.Gameplay
{
    public class Grid3D<TValue>
    {
        public int size => xSize * ySize * zSize;

        public int xSize;
        public int ySize;
        public int zSize;

        public TValue[] grid;
        public TValue emptyCell;

        public TValue this[int index] {
            get => Get(index);
            set => Set(index, value);
        }

        public TValue this[int x, int y, int z] {
            get => Get(x, y, z);
            set => Set(x, y, z, value);
        }

        public Grid3D(int xSize, int ySize, int zSize, TValue emptyCell) {
            this.xSize = xSize;
            this.ySize = ySize;
            this.zSize = zSize;
            grid = new TValue[size];
            this.emptyCell = emptyCell;
            Fill(emptyCell);
        }

        public void Fill(TValue value) {
            for (int i = 0; i < size; i++) {
                grid[i] = value;
            }
        }

        public void Set(int index, TValue value) {
            grid[index] = value;
        }

        public void Set(int x, int y, int z, TValue value) {
            Set(Index(x, y, z), value);
        }

        public bool SetSafe(int index, TValue value) {
            if (index > 0 && index < (xSize * ySize)) {
                Set(index, value);
                return true;
            }

            return false;
        }

        public bool SetSafe(int x, int y, int z, TValue value) {
            return SetSafe(Index(x, y, z), value);
        }

        public TValue Get(int index) {
            return grid[index];
        }

        public TValue Get(int x, int y, int z) {
            return Get(Index(x, y, z));
        }

        public bool GetSafe(int index, out TValue value) {
            if (index > 0 && index < (xSize * ySize)) {
                value = Get(index);
                return true;
            }

            value = default;
            return false;
        }

        public bool GetSafe(int x, int y, int z, out TValue value) {
            return GetSafe(Index(x, y, z), out value);
        }

        public int Index(int x, int y, int z) {
            return (z * xSize * ySize) + (y * ySize) + x;
        }

        public void Position(int index, out int x, out int y, out int z) {
            z = index / (xSize * ySize);
            index -= z * xSize * ySize;
            y = index / xSize;
            x = index % xSize;
        }

        public UnityEngine.Vector3Int Position(int index) {
            int x, y, z;
            Position(index, out x, out y, out z);
            return new UnityEngine.Vector3Int(x, y, z);
        }

        public TValue[] GetNeighborsXY(int index, int z) {
            TValue[] result = new TValue[4];
            GetNeighborsXYNoAlloc(index, z, result);
            return result;
        }

        public void GetNeighborsXYNoAlloc(int index, int z, TValue[] result) {
            int x, y;
            Position(index, out x, out y, out z);

            // North
            if (y >= (ySize - 1)) result[0] = emptyCell;
            else result[0] = grid[Index(x, y + 1, z)];

            // East
            if (x >= (xSize - 1)) result[1] = emptyCell;
            else result[1] = grid[Index(x + 1, y, z)];

            // South
            if (y <= 0) result[2] = emptyCell;
            else result[2] = grid[Index(x, y - 1, z)];

            // West
            if (x <= 0) result[3] = emptyCell;
            else result[3] = grid[Index(x - 1, y, z)];
        }
    }
}