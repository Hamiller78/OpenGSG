using System.Collections.Generic;
using System.Windows.Forms;
using WorldData;
using Military;

namespace ColdWarPrototype2.Views
{
    /// <summary>
    /// Simple view helper for showing armies in a province.
    /// Provides methods to get armies for a province and to fill a ListBox control.
    /// </summary>
    public class ArmyList
    {
        private readonly Form1 motherWindow_;

        public ArmyList(Form1 motherWindow)
        {
            motherWindow_ = motherWindow;
        }

        /// <summary>
        /// Returns the list of armies present in the given province from the provided world state.
        /// </summary>
        public List<Army> GetArmiesInProvince(WorldState state, int provinceId)
        {
            var mgr = state?.GetArmyManager();
            if (mgr == null) return new List<Army>();
            return mgr.GetArmiesInProvince(provinceId);
        }

        /// <summary>
        /// Fills the given ListBox control with the armies in the specified province.
        /// Each army's ToString() is used for display.
        /// </summary>
        public void FillListBox(ListBox listBox, WorldState state, int provinceId)
        {
            listBox.Items.Clear();
            var armies = GetArmiesInProvince(state, provinceId);
            foreach (var army in armies)
            {
                listBox.Items.Add(army);
            }
        }
    }
}
