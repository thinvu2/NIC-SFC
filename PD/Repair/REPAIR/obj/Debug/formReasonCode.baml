   M S B A M L   `   `   `  ����A  =REPAIR, Version=1.0.4.6, Culture=neutral, PublicKeyToken=nullclr-namespace:REPAIRREPAIR  5   	   C&clr-namespace:REPAIR.UserControlRepairREPAIR.UserControlRepair  5   	       REPAIR.formReasonCode   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:REPAIR     5   	   5uc&clr-namespace:REPAIR.UserControlRepair     5   	   J FXamlAnimatedGif, Version=1.2.3.0, Culture=neutral, PublicKeyToken=null<gif2https://github.com/XamlAnimatedGif/XamlAnimatedGif  5   	   W SMaterialDesignThemes.Wpf, Version=1.2.0.27835, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5
   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5   	   -   5        $��frmReasonCode��5   	   $��Regular"�5   	   $��16(�5   	   &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�5   	      TextRenderingMode$
 Auto=�5   	    � WindowStyle$
 None=�5   	    � Title$ Reason Code��5   	   $	��400��6   $	��700��6*    � WindowStartupLocation$ CenterScreen=�6Q    � 	Resources 5         
Label_text �� Color  	Colortext  btncolor  Icon  Text 	 Text2 	 Text3 �� 
TargetType �� Property	 �� Value:  4System.Windows.Controls.Primitives.DataGridRowHeader��%  &	        &	 !     &	 D     &	 g     &	 �     &	 �     &	 �     '�� �     '�� �    '�� �    '�� ^    '  '    �� 5   
   $ #0889a6��6-   �� 5   
   $ 	OrangeRed��6,   �� 5   
   $ 	#FFBF4328��6+   �� 5   
   $ White��6'   �� 5   
   $ White��6'   �� 5   
   $ 	#FF1B0303��6(   �� 5   
   $	 Red��6(   �� 5   
   " ��6   .�5      ��  � ��	 �#09345d6+   �� 5       � ��$	 18(�6)   �� 5       � ��	 �#FFF6+   �� 5       � ��$	 1q�60   �� 5        � ��$	 Center=�62   6C   5!      �� 5"   
   " ��6   .�5#      ��  � ��	 �#09345d6+   �� 5$       � ��$	 18(�6)   �� 5%       � ��	 �#FFF6+   �� 5&       � ��$	 Center=�62   6C   5'      �� 5(   
   " ��6   .�5)      ��  � ��	 �#09345d6+   �� 5*       � ��	 �#FFF6+   �� 5+       � ��$	 Center=�62   6C   �� 5,       � ��$	 0q�60   6<   5-      �� 5.   
   " ��6   .�5/      ��  � ��$	 Center=�62   �� 50       � ��	 �	#FF0807076+   �� 51       � ��$	 Right=�64   �� 52       � ��$	 0 0 10 0q�6'   53      �� 55   
   "  6   .�56      ��  � ��$		 5 0q�6'   �� 57       � ��$	 Center=�6;   58      59      .��5:      � ���	#FF117063
 � RowDefinitions
 5;   
   �� 5<      $
��auto �6   6,   �� 5=      6   .+�5?   
   � $��0��6   .+�5@      $   REPAIR.UserControlRepair.ucBar  
 � Tag 6   �� *Title+ �� ElementName$ frmReasonCode��6L   5A      � 5B   
   $��1��6   
 5C      �� 5D      $��40 �6    6.   �� 5E      6"   .+�5G      � $��0��6   .+�5H      � $��Center=�6   $��Right=�67   .+�5I      �� -   5        $��tbInput��5I   *   $	��200��6   $��35��6:   6_   �� 5J      -   5        $��	btnSelect��5J      $	��100��6.   $
��10 0q�6;   #��[ 6I   #��[ 6p   .��5K      �  � ColumnDefinitions 5L      �� 5M   "   66   �� 5N   "   66   .+�5P      � $��1��6#   $��Center=�64   $
��Left=�6O   .+�5Q   "   �� $��Select��6(   
�������$��-13,-2,-34,4q�6L   $��27��6b   6s   5R      � 5S      $��0��6#   $��Center=�63   $��Center=�6N   .+�5T   "   '  !MaterialDesignThemes.Wpf.PackIcon  �  �ControlzEx.PackIconBase`1[[MaterialDesignThemes.Wpf.PackIconKind, MaterialDesignThemes.Wpf, Version=1.2.0.27835, Culture=neutral, PublicKeyToken=null]]   Kind$ FileFind=�6:   $��30��6K   $��38��6W   
������� J� RenderTransformOrigin$ 0.5,0.5&�6u   $��-44,-6,0,-5q�6�   z�5U   &   V� 5V   *   .~�5W   .   �� �� 5X   .   �� 5Y   .    �� Angle$ 0.406Q�6>   T� 5Z   .   5[   +   5]   #   5^      5_      5`      5a      5b      � 5�      $��1��6   $��5q�6    .+�5�      &   System.Windows.Controls.DataGrid  -   5        $��gridResonCode��5�      $��0��60   $��3��6>      CanUserAddRows .     VerticalGridLinesBrush �	#FF4D4747   HorizontalGridLinesBrush �	#FF4D4747   GridLinesVisibility$	 All=�5�   R   4  .System.Windows.Automation.AutomationProperties   IsColumnHeader .    
IsReadOnly .   �� IsTextSearchCaseSensitive . ��5�   S   ��  5�      =  7System.Windows.Controls.Primitives.DataGridColumnHeader��%�   '        �� 5�      "  6    .�5�      ��  � ��	 �	#FFF7E2B56O   �� 5�       � ��$	 0 0 1 1q�6@   6R   �� 5�       � ��	 �	#FF4D47476P   �� 5�       � ��$	 30��67   6D   5�      5�      ��5�      �� 5�       �� EndPoint$ 0.5,1&�6.    �� 
StartPoint$ 0.5,0&�6?   .��5�      �  � Color$ Black��6+    � Offset$ 0Q�69   � 5�      $ White��6+   $ 1Q�69   5�      5�      5�      5�      5�      5�      