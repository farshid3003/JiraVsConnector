namespace gadget {
    internal class Filter {
        public readonly string Name;
        public readonly string FilterDef;

        public Filter(string name, string filterDef) {
            Name = name;
            FilterDef = filterDef;
        }
    }
}
