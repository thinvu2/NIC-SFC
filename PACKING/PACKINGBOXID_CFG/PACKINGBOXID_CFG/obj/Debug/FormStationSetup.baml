   M S B A M L   `   `   `  ����G  CPackingBoxID, Version=1.0.4.2, Culture=neutral, PublicKeyToken=null3clr-namespace:PACKINGBOXID_CFGPACKINGBOXID_CFG  5   	   '    !PACKINGBOXID_CFG.FormStationSetup   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:PACKINGBOXID_CFG     5   	   U QMaterialDesignThemes.Wpf, Version=2.3.1.953, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	   Bi:http://schemas.microsoft.com/expression/2010/interactivity  5	   	     � WindowStartupLocation$  CenterScreen=�5
   	      MaterialDesignBody#���   5   	   $��Regular"�5   	   $��16(�5   	   &   System.Windows.Media.TextOptions   TextFormattingMode$ Ideal=�5   	      TextRenderingMode$
 Auto=�5   	   $��Arial)�5   	     MaterialDesignPaper#���  5   	    � Title$ Station_Setup��5   	   $	��550��6   $	��500��6,   .��5      �  � 	Resources 5   
    �� 
TargetType 	 Title �� Property �� Value��%�   &	       �� 5      " ��6   .�5      ��  �@��Padding$ 8q�6,   �� 5       � ��$ 25��6+   �� 5       � ��
 �����6/   �� 5       � ��$ Center=�68   5      5      ��5   
   �� 5       �� EndPoint$ 	0.504,1.5&�6"   	 �� 
StartPoint$	 
0.504,0.03&�5   "   .��5      � 
 � Color$
 #034E70��6    � Offset$ 0Q�6/   � 5       $
 #0889A6��6   $ 0.567Q�6/   5!       � RowDefinitions 5#   
   �� 5$      $��1* �6   �� 5%      $��2* �6   �� 5&      $��4* �6   �� 5'      $��1* �6   .+�5)   
   � $��0��6   $
��40 0q�6    � ColumnDefinitions 5*      �� 5+      $��* �6#   �� 5,      $��* �6#   .+�5.      � $��0��6    5/      �� 50      $��1* �6'   �� 51      $��2* �6'   .+�53      � $��0��6   $��Right=�6'   .+�54      �� -   5         ��Name$ cb_change_line��54      
�������
�������
�������$��25��6q    55        MaterialDesignCheckBoxOff �� Color��%)   &	       �� 56      $ White��6P   57      58      59      � 5:      $��1��6   .+�5;      �� #��[ 6    $��Change Line��6?    �� Padding$ 4q�6R   5<      5=      � 5>      $��1��6   $��Center=�6#   .+�5?      #  MaterialDesignThemes.Wpf.Card  .��5@      �� -   5         ��Name$ cbb_line_name��5@      $��25��66   
��� ��� �� 
IsReadOnly .  �� 
IsEditable . $��5q�6z   5A      5B      5C      � 5D   
   $��1��6   $
��40 0q�6    5E      �� 5F      $��* �6#   �� 5G      $��* �6#   .+�5I      � $��0��6   $��0 0 60 0q�6#    5J      �� 5K      �� 5L      �� 5M      .+�5O      � $��0��6   .+�5P      �� #��[ 6    $��	Section :��6?   5Q      � 5R      $��1��6   .+�5S      �� #��[ 6    $��Group :��6?   5T      � 5U      $��2��6   .+�5V      �� #��[ 6    $��	Station :��6?   5W      5X      � 5Y      $��1��6    5Z      �� 5[      �� 5\      �� 5]      .+�5_      � $��0��6   $��Center=�6$   .+�5`        .��5a      �� -   5         ��Name$ Edt_Section��5a   $   $��25��69   $ 5q�6E   5b      5c      � 5d      $��1��6   $��Center=�6$   .+�5e        .��5f      �� -   5        $ 	Edt_Group��5f   $   $��25��67   $ 5q�6C   5g      5h      � 5i      $��2��6   $��Center=�6$   .+�5j        .��5k      �� -   5        $ Edt_Station��5k   $   $��25��69   $ 5q�6E   5l      5m      5n      5o      � 5p   
   $��2��6   .+�5q      � $��10q�6   
�������.+�5r      &   System.Windows.Controls.DataGrid  -   5        $��DataGrid��5r         
IsReadOnly . 5t      5u      5v      � 5w   
   $��3��6    5x      �� 5y      �� 5z      .+�5|      � $��0��6   .+�5}      �� -   5        
���2͚�
���  ��$	��100��5}   Z   ��62   Z� .��5~      ��  �� Orientation$ 
Horizontal=�6!   .+�5      '  !MaterialDesignThemes.Wpf.PackIcon  �  �ControlzEx.PackIconBase`1[[MaterialDesignThemes.Wpf.PackIconKind, MaterialDesignThemes.Wpf, Version=2.3.1.953, Culture=neutral, PublicKeyToken=null]]   Kind$ Check=�62   �� 5�      $��OK��6$   60   5�      5�      5�      � 5�      $��1��6   .+�5�      �� -   5        
���2͚�
���  ��$	��100��5�   Z   ��62   Z� .��5�      �� $ 
Horizontal=�6!   .+�5�        $ Close=�62   �� 5�      $��Cancel��6$   5�      5�      5�      5�      5�      5�      