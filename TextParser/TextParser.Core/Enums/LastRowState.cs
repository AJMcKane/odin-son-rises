using System;
using System.Collections.Generic;
using System.Text;

namespace TextParser.Core.Enums {
    public enum RowState {
        Standard = 0,
        OneElementRegistration = 1,
        OneElementDateLease = 2,
        NoteElement = 3,
        TwoElementsRegistrationAndDate = 4,
        TwoElementsPropertyAndDate = 5,
    }
}
