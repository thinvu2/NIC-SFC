   M S B A M L   `   `   `  ����A  =REPAIR, Version=1.0.4.6, Culture=neutral, PublicKeyToken=nullclr-namespace:REPAIRREPAIR  5   	   C&clr-namespace:REPAIR.UserControlRepairREPAIR.UserControlRepair  5   	       REPAIR.formAddError   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:REPAIR     5   	   5uc&clr-namespace:REPAIR.UserControlRepair     5   	   J FXamlAnimatedGif, Version=1.2.3.0, Culture=neutral, PublicKeyToken=null<gif2https://github.com/XamlAnimatedGif/XamlAnimatedGif  5   	   W SMaterialDesignThemes.Wpf, Version=1.2.0.27835, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5
   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5   	   -   5        $��AddError��5   	   $��Regular"�5   	   $��16(�5   	   &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�5   	      TextRenderingMode$
 Auto=�5   	    � WindowStyle$
 None=�5   	    � Title$ Add a new error code��5   	   $	��400��6&   $	��700��63    � WindowStartupLocation$ CenterScreen=�6X    � 	Resources 5         
Label_text �� Color  	Colortext  btncolor  Icon  Text 	 Text2 	 Text3 �� 
TargetType �� Property	 �� Value:  4System.Windows.Controls.Primitives.DataGridRowHeader��%`  &	        &	 !     &	 D     &	 g     &	 �     &	 �     &	 �     '�� �     '�� �    '�� �    '�� �    '  w    �� 5   
   $ #0889a6��6-   �� 5   
   $ 	OrangeRed��6,   �� 5   
   $ 	#FFBF4328��6+   �� 5   
   $ White��6'   �� 5   
   $ White��6'   �� 5   
   $ 	#FF1B0303��6(   �� 5   
   $	 Red��6(   �� 5   
   " ��6   .�5      ��  � ��
	 �   �6+   �� 5       � ��$	 18(�6)   �� 5       � ��	 �#FFF6+   �� 5       � ��$	 3q�6(   64   �� 5        � ��$	 1q�60   �� 5!       � ��$	 Center=�62   6C   5"      �� 5#   
   " ��6   .�5$      ��  � ��
	 �   �6+   �� 5%       � ��$	 18(�6)   �� 5&       �@��Padding$	 3q�6(   64   �� 5'       � ��	 �#FFF6+   �� 5(       � ��$	 Center=�62   6C   5)      �� 5*   
   " ��6   .�5+      ��  � ��	 �#09345d6+   �� 5,       � ��	 �#FFF6+   �� 5-       � ��$	 Center=�62   6C   �� 5.       � ��$	 0q�60   6<   5/      �� 50   
   " ��6   .�51      ��  � ��$	 Center=�62   �� 52       � ��	 �	#FF0807076+   �� 53       � ��$	 Right=�64   �� 54       � ��$	 0 0 10 0q�6'   55      �� 57   
   "  6   .�58      ��  � ��$		 5 0q�6'   �� 59       � ��$	 Center=�6;   5:      5;      .��5<      � ���	#FF117063
 � RowDefinitions
 5=   
   �� 5>      $
��auto �6   6,   �� 5?      6   .+�5A   
   � $��0��6   .+�5B      $   REPAIR.UserControlRepair.ucBar  
 � Tag 6   �� *Title+ �� ElementName$ frmAddError��6J   5C      � 5D   
   $��1��6   
 5F      �� 5G      $	��100 �6    6/   �� 5H      6"   .+�5J      � $��0��6    � ColumnDefinitions 5K      �� 5L      �� 5M      $
��auto �6'   .+�5O      � $��0��6    5P      �� 5Q      $
��auto �6+   �� 5R      .+�5T      � $	��5 0q�6   
 5U      �� 5V      �� 5W      .+�5Y      �� $��Error Code :��6$   6:   �� 5Z      $��Description :��6$   $��1��69   6H   5[      � 5\      $��1��6   $
��20 0q�6+   
 5]      �� 5^      �� 5_      .+�5a      �� -   5        $��tbErrorCode��5a   "   
��� ���
���  ��$�� ��5b   3   $��	0 0 200 0q�5c   "   $	��150��5d   "   5e   %   �� 5f      -   5        $��tbDescription��5f   "   $��1��67   6F   5g      5h      �� 5j      $��Center=�6   $��Right=�68   $��1��6T   $��10q�6d   .+�5k      �� -   5        $��btnOK��5k      $��80��6*   $��5q�66   #��[ 6A   #��[ 6h   .��5l      �  5m      �� 5n   "   66   �� 5o   "   66   .+�5q      � $��1��6#   $��Center=�64   $
��Left=�6O   .+�5r   "   �� $��OK��6(   
�������$��-5,-4,-24,2q�6H   $��26��6]   6n   5s      � 5t      $��0��6#   $��Center=�63   $��Center=�6N   .+�5u   "   '  !MaterialDesignThemes.Wpf.PackIcon  �  �ControlzEx.PackIconBase`1[[MaterialDesignThemes.Wpf.PackIconKind, MaterialDesignThemes.Wpf, Version=1.2.0.27835, Culture=neutral, PublicKeyToken=null]]   Kind$ Check=�6:   $��30��6H   $��38��6T   
������� J� RenderTransformOrigin$ 0.5,0.5&�6r   $��-45,-6,0,-5q�6�   z�5v   &   V� 5w   *   .~�5x   .   �� �� 5y   .   �� 5z   .    �� Angle$ 0.406Q�6>   T� 5{   .   5|   +   5~   #   5      5�      5�      �� 5�      -   5        $��	btnCancel��5�      $��80��6.   $��5q�6:   #��[ 6E   #��[ 6l   .��5�      �  5�      �� 5�   "   66   �� 5�   "   66   .+�5�      � $��1��6#   $��Center=�64   $
��Left=�6O   .+�5�   "   �� $��Cancel��6(   
�������$��-13,-2,-34,4q�6L   $��27��6b   6s   5�      � 5�      $��0��6#   $��Center=�63   $��Center=�6N   .+�5�   "     $ Close=�6:   $��30��6H   $��38��6T   
�������$ 0.5,0.5&�6r   $��-44,-6,0,-5q�6�   z�5�   &   V� 5�   *   .~�5�   .   �� �� 5�   .   �� 5�   .   $ 0.406Q�6>   T� 5�   .   5�   +   5�   #   5�      5�      5�      5�      5�      � 5�      $��1��6   $��5q�6    .+�5�      &   System.Windows.Controls.DataGrid  -   5        $��gridErrorCode��5�      $��0��60   $��3��6>      CanUserAddRows .     VerticalGridLinesBrush �	#FF4D4747   HorizontalGridLinesBrush �	#FF4D4747   GridLinesVisibility$	 All=�5�   R   4  .System.Windows.Automation.AutomationProperties   IsColumnHeader .    
IsReadOnly .   �� IsTextSearchCaseSensitive . ��5�   S   ��  5�      =  7System.Windows.Controls.Primitives.DataGridColumnHeader��%�   '        �� 5�      "  6    .�5�      ��  � ��	 �	#FFF7E2B56O   �� 5�       � ��$	 0 0 1 1q�6@   6R   �� 5�       � ��	 �	#FF4D47476P   �� 5�       � ��$	 30��67   6D   5�      5�      ��5�      �� 5�       �� EndPoint$ 0.5,1&�6.    �� 
StartPoint$ 0.5,0&�6?   .��5�      �  � Color$ Black��6+    � Offset$ 0Q�69   � 5�      $ White��6+   $ 1Q�69   5�      5�      5�      5�      5�      5�      