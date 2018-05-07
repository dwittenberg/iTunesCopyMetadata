using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TryAndError
{
    class MainViewModel
    {
        public ICollectionView SongStruct { get; private set; }
        public ICollectionView GroupedSongStruct { get; private set; }

        public MainViewModel()
        {
            var _songStruct = new List<SongStruct>
                                 {
                                     new SongStruct{
                                         Name = "Name"
                                     },
                                     new SongStruct{
                                         Name = "Namen"
                                     },
                                     new SongStruct{
                                         Name = "Namer"
                                     },
                                     new SongStruct{
                                         Name = "Names"
                                     },
                                 };

            SongStruct = CollectionViewSource.GetDefaultView(_songStruct);

            GroupedSongStruct = new ListCollectionView(_songStruct);
            GroupedSongStruct.GroupDescriptions.Add(new PropertyGroupDescription("Gender"));
        }
    }
}