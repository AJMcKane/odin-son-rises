﻿using System.Threading.Tasks;

namespace TextParser.DAL.Interfaces {
    public interface ITextParserDataService<T> where T : class {
        public Task Save(T item);
    }
}
