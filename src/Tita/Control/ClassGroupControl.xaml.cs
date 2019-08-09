﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tita
{
    public class EditEventArgs : EventArgs
    {
        public string newname { get; set; }
    }

    public class ClassGroupRemoveArgs : EventArgs
    {
        public ClassGroup rootGroup { get; set; }
    }

    public class ClassChangeMemberEventArgs : EventArgs
    {
        public ClassGroup rootGroup { get; set; }
        public ClassInfo changeInfo { get; set; }
        public int add_delete {get; set;}
    }

    /// <summary>
    /// ClassGroupControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ClassGroupControl : UserControl
    {
        public delegate void mydel(object sender, EditEventArgs e);
        public event mydel EditGroupName;
        //public event EventHandler<ClassRemoveArgs> ClassRemove;
        public event EventHandler<ClassGroupRemoveArgs> ClassGroupRemove;
        public event EventHandler<ClassChangeMemberEventArgs> ChangeMember;

        public ClassGroupControl()
        {
            InitializeComponent();
            Group = new ClassGroup();

            editbutton.Visibility = Visibility.Hidden;
            editname.Visibility = Visibility.Hidden;
        }

        public ClassGroup Group { get; set; }

        /// <summary>
        /// 새로운 그룹 추가
        /// </summary>
        /// <param name="group"></param>
        public ClassGroupControl(ClassGroup group) : this()
        {
            this.Group = group;
            BasketUpdate();
        }

        /// <summary>
        /// 그룹 최초 생성할 때 부르는 클래스
        /// </summary>
        /// <param name="group"></param>
        public void BasketUpdate()
        {
            foreach (IGroupable item in Group.Children)
            {
                if (Group.IsitGroup(item)) { }
                else
                {
                    ClassInfoControl groupitem = new ClassInfoControl((ClassInfoPlus)item);
                    groupitem.ClassRemove += ClassRemoveMember;
                    basketstack.Children.Add(groupitem);
                }
            }
        }

        private void DragSubject_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(nameof(ClassInfo)))
            {
                e.Effects = DragDropEffects.Move;
            }
        }

        private void Data_Drop(object sender, DragEventArgs e)
        {
            if (e.Handled == false)
            {
                StackPanel panel = sender as StackPanel;
                ClassInfo curinfo = e.Data.GetData(nameof(ClassInfo)) as ClassInfo;
                if (Subject_Add(curinfo)) return;
                Group.Children.Add(new ClassInfoPlus(curinfo));
                ClassInfoPlus infoplus = new ClassInfoPlus(curinfo);
                if (panel != null && curinfo != null)
                {

                    ClassInfoControl curcontrol = new ClassInfoControl(infoplus);
                    curcontrol.AllowDrop = false;
                    curcontrol.ClassRemove += ClassRemoveMember;
                    panel.Children.Add(curcontrol);
                    e.Effects = DragDropEffects.Move;
                }

            }
        }

        /// <summary>
        /// 새로운 과목이 들어오면 그룹에 접근해서 해당과목이 존재하는지 확인해주고 없을 경우 추가하는 이벤트를 위로 보내줌, 추가 : 1
        /// </summary>
        /// <param name="Info"></param>
        private bool Subject_Add(ClassInfo Info)
        {
            foreach(IGroupable g in Group.Children)
            {
                ClassInfoPlus g_plus = g as ClassInfoPlus;
                if (Info == g_plus.Info) return true;
            }
            ClassChangeMemberEventArgs changeargs = new ClassChangeMemberEventArgs();
            changeargs.rootGroup = Group;
            changeargs.changeInfo = Info;
            changeargs.add_delete = 1;
            ChangeMember?.Invoke(this,changeargs);  //info가 추가 되었을 때 추가 : 1
            return false;
        }


        private void penClick(object sender, RoutedEventArgs e)
        {
            groupname.Visibility = Visibility.Hidden;
            penb.Visibility = Visibility.Hidden;
            editbutton.Visibility = Visibility.Visible;
            editname.Visibility = Visibility.Visible;

            String name = groupname.Text;
            editname.Text = name;
        }

        private void editClick(object sender, RoutedEventArgs e)
        {
            EditEventArgs argevent = new EditEventArgs();
            argevent.newname = editname.Text;

            if(EditGroupName != null)
            {
                EditGroupName(this, argevent);
            }
            //참고 EditGroupName?.Invoke(this, argevent); (?.은 앞의 변수가 null이면 무시)

            groupname.Text = argevent.newname;

            groupname.Visibility = Visibility.Visible;
            penb.Visibility = Visibility.Visible;
            editbutton.Visibility = Visibility.Hidden;
            editname.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 그룹이 삭제 되었을 때 삭제 : 0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteClick(object sender, RoutedEventArgs e)
        {
            ClassGroupRemove?.Invoke(this, new ClassGroupRemoveArgs() { rootGroup = this.Group});

            /*
            ClassChangeGroupEventArgs changeargs = new ClassChangeGroupEventArgs();
            changeargs.rootGroup = Group;
            changeargs.add_delete = 0;
            ChangeGroup?.Invoke(this, changeargs);
            */
        }

        /// <summary>
        /// member 삭제되었을 때 삭제 : 0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="re"></param>
        private void ClassRemoveMember(Object sender, ClassRemoveArgs re)
        {
            basketstack.Children.Remove(sender as ClassInfoControl);  //상위클래스에서 처리해주어야 할 일
            ClassChangeMemberEventArgs changeargs = new ClassChangeMemberEventArgs();
            changeargs.rootGroup = Group;
            changeargs.changeInfo = re.Info;
            changeargs.add_delete = 0;
            ChangeMember?.Invoke(this, changeargs);
        }
    }
}
