   M S B A M L   `   `   `  ����A  =REPAIR, Version=1.0.4.6, Culture=neutral, PublicKeyToken=nullC&clr-namespace:REPAIR.UserControlRepairREPAIR.UserControlRepair  5   	   clr-namespace:REPAIRREPAIR  5   	       REPAIR.MessageError   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   5uc&clr-namespace:REPAIR.UserControlRepair     5   	   &localclr-namespace:REPAIR     5   	   -   5        $��
MessageFrm��5
   	     � WindowState$  Normal=�5	   	    � WindowStyle$
 None=�6    � Title$ Thông báo lỗi��5   	   $	��250��6   $	��400��6,   $��Arial)�68   $��14(�6K    � WindowStartupLocation$ CenterScreen=�6v   .��5      �  � RowDefinitions 5   
   �� 5      $
��auto �6   6,   �� 5      6   .+�5   
   � $��0��6   .+�5      $   REPAIR.UserControlRepair.ucBar  
 � Tag 6   �� *Title+ �� ElementName$ 
MessageFrm��6I   5      � 5   
   $��1��6   ��5      �� 5       �� EndPoint$ 0.5,1&�6&    �� 
StartPoint$ 0.5,0&�67   .��5      � 	 � Color$	 	#FF409589��6#   
 � Offset$
 0.975Q�65   � 5      $	 	#FFC6E0E6��6#   $
 0.519Q�65   5       5      �� 5      6"   �� 5      6"   �� 5      6"   �� 5      6"   .+�5!      � $��0��6   .+�5"      ��  �� CornerRadius$ 10��6   ���	#FFCF2323$��1q�6C   $��5q�6W   
�������._�5#      �� -   5        $��
lblEnglish��5#       $��14(�62   
���  ��$
��Left=�6Q   $	��Top=�6l   $��WrapWithOverflow=�6�   $��5q�6�   5$      5%      � 5&      $��1��6   .+�5'      �� $ 10��6   ���	#FFCF2323$��1q�6C   $��5q�6W   
�������._�5(      �� -   5        $��lblVietNamese��5(       $��14(�65   
���  ��$
��Left=�6T   $	��Top=�6o   $��WrapWithOverflow=�6�   $��5q�6�   5)      5*      � 5+      $��2��6   .+�5,      �� -   5        $��Label_IP��5,      $��10.220.XX.XX��6(   $��13(�6@   
����  �$��Center=�6a   $��Center=�6|   $
��Bold"�6�   5-      � 5.      $��3��6    � ColumnDefinitions 5/      �� 50      6)   �� 51      6)   .+�53      � $��0��6   .+�54      G� -   5        $��password��54   "   $	��100��62   
���   �$��18(�6Q    G� PasswordChar$ *��6_   
�������$��40��6�   $��Right=�6�   $��0 0 20 0q�6�   6�   55      �� 57      -   5        $��btnOK��57      $
��Left=�6'   $��1��6B    #  MaterialDesignRaisedLightButton#��[  6R    � ToolTip$4 .Resource name: MaterialDesignRaisedLightButton��6�   $��OK��6�   $��20(�6�   ���	#FF7349C1$��1q�6�   
����������	#FFF26A1D$��35��58   o   $	��120��6{   5:      5<      5>      5?      