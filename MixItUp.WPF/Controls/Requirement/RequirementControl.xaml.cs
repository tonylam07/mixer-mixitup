﻿using MixItUp.Base.ViewModel.Requirement;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MixItUp.WPF.Controls.Requirement
{
    /// <summary>
    /// Interaction logic for RequirementControl.xaml
    /// </summary>
    public partial class RequirementControl : UserControl
    {
        public RequirementControl()
        {
            InitializeComponent();
        }

        public void HideCooldownRequirement()
        {
            this.CooldownPopup.Visibility = Visibility.Collapsed;
        }

        public void HideCurrencyRequirement()
        {
            this.CurrencyPopup.Visibility = Visibility.Collapsed;
        }

        public void HideThresholdRequirement()
        {
            this.ThresholdPopup.Visibility = Visibility.Collapsed;
        }

        public async Task<bool> Validate()
        {
            if (this.CurrencyPopup.Visibility == Visibility.Visible)
            {
                return await this.CooldownRequirement.Validate() && await this.CurrencyRequirement.Validate() && await this.RankRequirement.Validate() && await this.ThresholdRequirement.Validate();
            }
            else
            {
                return await this.CooldownRequirement.Validate() && await this.RankRequirement.Validate() && await this.ThresholdRequirement.Validate();
            }
        }

        public RequirementViewModel GetRequirements()
        {
            RequirementViewModel requirement = new RequirementViewModel();
            requirement.Role = this.RoleRequirement.GetRoleRequirement();
            requirement.Cooldown = this.CooldownRequirement.GetCooldownRequirement();
            if (this.CurrencyPopup.Visibility == Visibility.Visible)
            {
                requirement.Currency = this.CurrencyRequirement.GetCurrencyRequirement();
            }
            requirement.Rank = this.RankRequirement.GetCurrencyRequirement();
            requirement.Threshold = this.ThresholdRequirement.GetThresholdRequirement();
            return requirement;
        }

        public void SetRequirements(RequirementViewModel requirement)
        {
            this.RoleRequirement.SetRoleRequirement(requirement.Role);
            this.CooldownRequirement.SetCooldownRequirement(requirement.Cooldown);
            if (this.CurrencyPopup.Visibility == Visibility.Visible)
            {
                this.CurrencyRequirement.SetCurrencyRequirement(requirement.Currency);
            }
            this.RankRequirement.SetCurrencyRequirement(requirement.Rank);
            this.ThresholdRequirement.SetThresholdRequirement(requirement.Threshold);
        }

        private void UsageRequirementsHelpButton_Click(object sender, RoutedEventArgs e) { Process.Start("https://github.com/SaviorXTanren/mixer-mixitup/wiki/Usage-Requirements"); }
    }
}
