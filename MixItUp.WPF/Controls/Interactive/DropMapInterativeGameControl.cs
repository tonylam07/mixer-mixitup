﻿using Mixer.Base.Model.Interactive;
using Mixer.Base.Model.User;
using MixItUp.Base;
using MixItUp.Base.MixerAPI;
using MixItUp.Base.ViewModel.User;
using MixItUp.WPF.Controls.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MixItUp.WPF.Controls.Interactive
{
    public abstract class DropMapInterativeGameControl : CustomInteractiveGameControl
    {
        protected int maxTime;

        protected InteractiveConnectedSceneModel scene;
        protected InteractiveConnectedButtonControlModel positionButton;
        protected InteractiveConnectedButtonControlModel winnerButton;

        private Dictionary<uint, UserProfileAvatarControl> userAvatars = new Dictionary<uint, UserProfileAvatarControl>();
        private Dictionary<uint, Point> userPoints = new Dictionary<uint, Point>();

        private Canvas canvas;

        public DropMapInterativeGameControl(InteractiveGameModel game, InteractiveGameVersionModel version) : base(game, version) { }

        public void Initialize(Canvas canvas)
        {
            this.canvas = canvas;
        }

        protected abstract void UpdateTimerUI(int timeLeft);

        protected abstract string ComputeLocation(Point point);

        protected abstract Task UpdateWinnerUI(uint winner, string username, string location);

        protected override async Task GameConnectedInternal()
        {
            InteractiveConnectedSceneGroupCollectionModel sceneGroups = await ChannelSession.Interactive.GetScenes();
            if (sceneGroups != null)
            {
                this.scene = sceneGroups.scenes.FirstOrDefault();
                if (this.scene != null)
                {
                    this.positionButton = this.scene.buttons.FirstOrDefault(c => c.controlID.Equals("position"));
                    this.winnerButton = this.scene.buttons.FirstOrDefault(c => c.controlID.Equals("winner"));
                }
            }

            this.userAvatars.Clear();
            this.userPoints.Clear();
            this.canvas.Children.Clear();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () =>
            {
                if (this.scene != null && this.winnerButton != null)
                {
                    InteractiveControlModel control = null;

                    for (int i = 0; i < (maxTime + 1); i++)
                    {
                        await Task.Delay(1000);

                        int timeLeft = maxTime - i;

                        this.Dispatcher.Invoke(() =>
                        {
                            this.UpdateTimerUI(timeLeft);
                        });

                        control = new InteractiveControlModel() { controlID = this.winnerButton.controlID };
                        control.meta["timeleft"] = timeLeft;
                        await ChannelSession.Interactive.UpdateControls(this.scene, new List<InteractiveControlModel>() { control });
                    }

                    if (this.userPoints.Count > 0)
                    {
                        var users = this.userPoints.Keys.ToList();
                        Random random = new Random();
                        int index = random.Next(users.Count);
                        uint winner = users[index];
                        Point point = this.userPoints[winner];
                        UserProfileAvatarControl avatar = this.userAvatars[winner];

                        UserModel user = await ChannelSession.Connection.GetUser(winner);

                        string username = (user != null) ? user.username : "Unknown";
                        string location = this.ComputeLocation(point);

                        this.Dispatcher.InvokeAsync(async () =>
                        {
                            await this.UpdateWinnerUI(winner, username, location);
                            this.canvas.Children.Clear();
                            this.canvas.Children.Add(avatar);
                        });

                        control = new InteractiveControlModel() { controlID = this.winnerButton.controlID };
                        control.meta["userID"] = winner;
                        control.meta["username"] = username;
                        control.meta["location"] = location;
                        await ChannelSession.Interactive.UpdateControls(this.scene, new List<InteractiveControlModel>() { control });

                        await ChannelSession.Chat.SendMessage(string.Format("Winner: @{0}, Drop Location: {1}", username, location));
                    }
                }
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        protected override async Task OnInteractiveControlUsed(UserViewModel user, InteractiveGiveInputModel input, InteractiveConnectedControlCommand command)
        {
            if (input.input.controlID.Equals("position") && user != null && input.input.meta.ContainsKey("x") && input.input.meta.ContainsKey("y"))
            {
                if (this.scene != null && this.positionButton != null)
                {
                    Point point = new Point() { X = (double)input.input.meta["x"], Y = (double)input.input.meta["y"] };

                    this.userPoints[user.ID] = point;

                    InteractiveControlModel control = new InteractiveControlModel() { controlID = this.positionButton.controlID };
                    control.meta["userID"] = user.ID;
                    control.meta["x"] = point.X;
                    control.meta["y"] = point.Y;
                    await ChannelSession.Interactive.UpdateControls(this.scene, new List<InteractiveControlModel>() { control });

                    await this.Dispatcher.InvokeAsync(async () =>
                    {
                        UserProfileAvatarControl avatarControl = null;

                        if (!this.userAvatars.ContainsKey(user.ID))
                        {
                            avatarControl = new UserProfileAvatarControl();
                            await avatarControl.SetImageUrl("https://mixer.com/api/v1/users/" + user.ID + "/avatar");
                            avatarControl.SetSize(20);

                            this.canvas.Children.Add(avatarControl);
                            this.userAvatars[user.ID] = avatarControl;
                        }

                        avatarControl = this.userAvatars[user.ID];

                        double canvasX = ((point.X / 100.0) * this.canvas.Width);
                        double canvasY = ((point.Y / 100.0) * this.canvas.Height);

                        Canvas.SetLeft(avatarControl, canvasX - 10);
                        Canvas.SetTop(avatarControl, canvasY - 10);
                    });
                }
            }
        }
    }
}
