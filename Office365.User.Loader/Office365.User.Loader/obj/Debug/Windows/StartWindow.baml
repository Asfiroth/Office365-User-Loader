   M S B A M L   `   `   `  ����P  LOffice365.User.Loader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:Office365.User.Loader.WindowsOffice365.User.Loader.Windows  5   	   /    )Office365.User.Loader.Windows.StartWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   =local+clr-namespace:Office365.User.Loader.Windows     5   	     � Title$  StartWindow��5   	   $	��300��6   $	��500��6*    � Icon$ /Images/logo.png��5	   	    � WindowStartupLocation$ CenterScreen=�5
   	    � WindowStyle$
 None=�6.    � 
ResizeMode$ NoResize=�5   	   
���   �.��5      �  � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��0.2* �6   .+�5   
   �� $��0��6   $��0��6   $��40q�6-   $��/Images/logo.png��69   �� 5   
   $��1��6   $��0��6$   $%��office 365 users - Batch Loader��5      
�������$��24(�5      $��Center=�5      $
��Wrap=�5         InitTextBlock#��[  5      $
��0,40q�5      � 5   
   $��1��6   $��0��6   $��2��6,    5       �� 5!      $��* �6    �� 5"      $��* �6    .+�5$      �� -   5         ��Name$ Loader��5$      $��Center=�6(   $��15(�6E   #��[  6S   
�������6z   5%      5&      5'      