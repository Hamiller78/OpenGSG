using System.Collections.Generic;
using System.Windows.Forms;
using Military;
using WorldData;

namespace ColdWarPrototype2.Views
{
    /// <summary>
    /// Simple view helper for showing armies in a province.
    /// Provides methods to get armies for a province and to fill a ListBox control.
    /// Also supports updating the internal motherWindow's ArmyListBox like the original VB view.
    /// </summary>
    public class ArmyList
    {
        private readonly MainWindow motherWindow_;

        private int currentProvinceId_;
        private bool isChoosingTarget_ = false;
        private List<Army> armiesInProvince_ = new List<Army>();
        private List<Army> selectedArmies_ = new List<Army>();

        public ArmyList(MainWindow motherWindow)
        {
            motherWindow_ = motherWindow;
        }

        /// <summary>
        /// Returns the list of armies present in the given province from the provided world state.
        /// </summary>
        public List<Army> GetArmiesInProvince(WorldState state, int provinceId)
        {
            var mgr = state?.GetArmyManager();
            if (mgr == null)
                return new List<Army>();
            return mgr.GetArmiesInProvince(provinceId) ?? new List<Army>();
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

        /// <summary>
        /// Updates the mother window's ArmyListBox with armies from the given province.
        /// Mirrors the original VB behaviour which used BeginUpdate/EndUpdate and stored internal state.
        /// </summary>
        public void UpdateArmyListBox(WorldState currentState, int mouseProvinceId)
        {
            armiesInProvince_ = currentState?.GetArmyManager()?.GetArmiesInProvince(mouseProvinceId) ?? new List<Army>();

            if (motherWindow_?.ArmyListBox == null)
                return;

            var lb = motherWindow_.ArmyListBox;
            lb.Items.Clear();
            lb.BeginUpdate();
            if (armiesInProvince_ != null)
            {
                foreach (var army in armiesInProvince_)
                {
                    lb.Items.Add(army.ToString());
                }
            }
            lb.EndUpdate();
        }
    }
}
