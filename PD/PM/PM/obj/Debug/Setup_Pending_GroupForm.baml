   M S B A M L   `   `   `  ����=  9PM, Version=1.0.1.9, Culture=neutral, PublicKeyToken=null#clr-namespace:PM.ModelPM.Model  5   	        PM.Setup_Pending_GroupForm   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'selfclr-namespace:PM.Model     5   	   U QMaterialDesignThemes.Wpf, Version=2.3.1.953, Culture=neutral, PublicKeyToken=nullFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	   -   5        $��13(�5   	   $��Arial)�5	   	   $
��Bold"�5
   	     � WindowStartupLocation$  CenterScreen=�5   	    � Title$ Setup_Pending_GroupForm��5   	   $	��450��6)   $	��800��66   .��5      �  � RowDefinitions 5   
   �� 5      $��6* �6   �� 5      $��1* �6   .+�5   
   � $��0��6   $��3q�6    � ColumnDefinitions 5      �� 5      $��1* �6#   �� 5      $
��0.5* �6#   �� 5      $
��0.5* �6#   .+�5      � $��0��6   .+�5      R� -   5         R�Name$ Treeview_Group��5       � 	Resources 5       �� DataType   PM.Model.Root �� ItemsSource �� Orientation   PM.Model.Family   PM.Model.FamilyMemebr��%/  (��       5   3   *, +)(��     5"   3   *, +)(�� �    5(   '   *, +)�� 5      "  63    6Q   �� *	ItemsRoot+.R�5      �� $ 
Horizontal=�6)   .+�5   "   �� $��30��6(   $.��(/Asset/icons8-thick-vertical-line-48.png��63   �� 5   "   $��5q�6G   
����  ���6,   �� *	TitleRoot+6R   5       5!      �� 5"      "  63    6S   �� *	Members+.R�5#      �� $ 
Horizontal=�6)   .+�5$   "   �� $��20��6(   $ ��/Asset/icons8-route-48.png��63   �� 5%   "   $��5q�6C   
����  ���6,   �� *Title+6N   5&      5'      �� 5(      "  6'   .R�5)      �� $ 
Horizontal=�6)   .+�5*   "   �� $��20��6(   $2��,/Asset/icons8-red-triangle-pointed-up-48.png��63   �� 5+   "   $��5q�6B   
����  ���6,   �� *Name+6M   5,      5-      5.      5/      50      � 51      $��1��6   .+�52      �� -   5        	 ��Name$	 LstSectionName��52      
����  ���53      �� 54      .R�55      ]� ��. .+�56   "   � $	��Top=�6'   ���#008082 57   &   �� 58   *   6=   .+�5:   &   �� $��0��6,   
��� ���$��Center=�6P   .��6n   SectionName6{   5;   #   �� 5<   "   63   5=      5>      5@      5A      � 5B      $��2��6   .+�5C      �� -   5        $	 LstGroupName��5C      
����  ���5D      �� 5E      .R�5F      ]� ��. .+�5G   "   � $	��Top=�6'   ���#008082 5H   &   �� 5I   *   6=   .+�5K   &   �� $��0��6,   
��� ���$��Center=�6P   .��6n   	GroupName6y   5L   #   �� 5M   "   63   5N      5O      5Q      5R      5S      � 5T   
   $��1��6   ���#008082 5U      �� 5V      �� 5W      .+�5Y      � $��0��6   .+�5Z      �� -   5        ���#FFF$��Center=�5Z   B   $��Center=�6_   $��90��6z   ��6+   Z� ��5[      �� 5\      $ 
Horizontal=�6%   .+�5]      '  !MaterialDesignThemes.Wpf.PackIcon  ���#008082$��Center=�6K   �  �ControlzEx.PackIconBase`1[[MaterialDesignThemes.Wpf.PackIconKind, MaterialDesignThemes.Wpf, Version=2.3.1.953, Culture=neutral, PublicKeyToken=null]]
   Kind$
 Check=�6f   �� 5^      $��OK��6$   ���#00808261   5_      5a      5b      � 5c      $��1��6   .+�5d      �� -   5        ���#FFF$��Center=�5d   B   $��Center=�6_   $��90��6z   ��6+   Z� ��5e      �� 5f      $ 
Horizontal=�6%   .+�5g        
���  ��$��Center=�6G   $
 Close=�6b   �� 5h      $��Cancel��6$   ���#00808265   5i      5k      5l      5m      5n      5o      