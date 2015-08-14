Imports System.Collections.Generic

Imports Mogre


#Region "IcoSphere"
''' <summary>
''' IcoSphere class
''' <remarks>See http://blog.andreaskahler.com/2009/06/creating-icosphere-mesh-in-code.html </remarks>
''' </summary>
Public Class IcoSphere
    Public Class TriangleIndices
        Public v1 As Integer
        Public v2 As Integer
        Public v3 As Integer

        Public Sub New(ByVal _v1 As Integer, ByVal _v2 As Integer, ByVal _v3 As Integer)
            v1 = _v1
            v2 = _v2
            v3 = _v3
        End Sub

        Public Shared Operator <(ByVal ImpliedObject As TriangleIndices, ByVal o As TriangleIndices) As Boolean
            Return (ImpliedObject.v1 > o.v1) AndAlso (ImpliedObject.v2 < o.v2) AndAlso (ImpliedObject.v3 < o.v3)
        End Operator
        Public Shared Operator >(ByVal ImpliedObject As TriangleIndices, ByVal o As TriangleIndices) As Boolean
            Return (ImpliedObject.v1 > o.v1) AndAlso (ImpliedObject.v2 > o.v2) AndAlso (ImpliedObject.v3 > o.v3)
        End Operator
    End Class

    Public Class LineIndices
        Public v1 As Integer
        Public v2 As Integer

        Public Sub New(ByVal _v1 As Integer, ByVal _v2 As Integer)
            v1 = _v1
            v2 = _v2
        End Sub

        Public Shared Operator =(ByVal ImpliedObject As LineIndices, ByVal o As LineIndices) As Boolean
            Return (ImpliedObject.v1 = o.v1 AndAlso ImpliedObject.v2 = o.v2) OrElse (ImpliedObject.v1 = o.v2 AndAlso ImpliedObject.v2 = o.v1)
        End Operator
        Public Shared Operator <>(ByVal ImpliedObject As LineIndices, ByVal o As LineIndices) As Boolean
            Return (ImpliedObject.v1 <> o.v1 AndAlso ImpliedObject.v2 <> o.v2) OrElse (ImpliedObject.v1 <> o.v2 AndAlso ImpliedObject.v2 <> o.v1)
        End Operator
        ' Equals & GetHashCode override, as guideline [http://msdn.microsoft.com/en-us/library/bsc2ak47.aspx]
        Public Overrides Function Equals(ByVal obj As System.Object) As Boolean
            ' If parameter is null return false.
            If obj Is Nothing Then
                Return False
            End If

            ' If parameter cannot be cast to Type return false.
            Dim p As LineIndices = TryCast(obj, LineIndices)
            If DirectCast(p, System.Object) Is Nothing Then
                Return False
            End If

            ' Return true if the fields match:
            Return (v1 = p.v1) AndAlso (v2 = p.v2)
        End Function
        Public Overloads Function Equals(ByVal p As LineIndices) As Boolean
            ' If parameter is null return false.
            If DirectCast(p, Object) Is Nothing Then
                Return False
            End If

            ' Return true if the fields match:
            Return (v1 = p.v1) AndAlso (v2 = p.v2)
        End Function
        Public Overrides Function GetHashCode() As Integer
            Return v1 Xor v2
        End Function

    End Class


    Private vertices As New List(Of Vector3)()
    Private _lineIndices As New LinkedList(Of LineIndices)()
    Private _triangleIndices As New LinkedList(Of Integer)()
    Private faces As New LinkedList(Of TriangleIndices)()
    Private middlePointIndexCache As New Dictionary(Of Long, Integer)()

    Private index As Integer


    Public Sub New()
        index = 0
    End Sub

    Public Sub Dispose()
        ' TODO: Verify & complete if necessary
        vertices.Clear()
        vertices = Nothing
        _lineIndices.Clear()
        _lineIndices = Nothing
        _triangleIndices.Clear()
        _triangleIndices = Nothing
        faces.Clear()
        faces = Nothing
        middlePointIndexCache.Clear()
        middlePointIndexCache = Nothing
    End Sub

    Public Sub Create(ByVal recursionLevel As Integer)
        vertices.Clear()
        _lineIndices.Clear()
        _triangleIndices.Clear()
        faces.Clear()
        middlePointIndexCache.Clear()
        index = 0

        ' Base Icosahedron creation
        Dim t As Single = (1.0F + Math.Sqrt(5.0F)) / 2.0F

        ' |1| create 12 vertices of a icosahedron
        AddVertex(New Vector3(-1.0F, t, 0.0F))
        ' Z = 0
        AddVertex(New Vector3(1.0F, t, 0.0F))
        AddVertex(New Vector3(-1.0F, -t, 0.0F))
        AddVertex(New Vector3(1.0F, -t, 0.0F))
        AddVertex(New Vector3(0.0F, -1.0F, t))
        ' X = 0
        AddVertex(New Vector3(0.0F, 1.0F, t))
        AddVertex(New Vector3(0.0F, -1.0F, -t))
        AddVertex(New Vector3(0.0F, 1.0F, -t))
        AddVertex(New Vector3(t, 0.0F, -1.0F))
        ' Y = 0
        AddVertex(New Vector3(t, 0.0F, 1.0F))
        AddVertex(New Vector3(-t, 0.0F, -1.0F))
        AddVertex(New Vector3(-t, 0.0F, 1.0F))
        ' |2| create 20 triangles (faces) of the icosahedron 
        AddFace(0, 11, 5)
        AddFace(0, 5, 1)
        AddFace(0, 1, 7)
        AddFace(0, 7, 10)
        AddFace(0, 10, 11)

        AddFace(1, 5, 9)
        AddFace(5, 11, 4)
        AddFace(11, 10, 2)
        AddFace(10, 7, 6)
        AddFace(7, 1, 8)

        AddFace(3, 9, 4)
        AddFace(3, 4, 2)
        AddFace(3, 2, 6)
        AddFace(3, 6, 8)
        AddFace(3, 8, 9)

        AddFace(4, 9, 5)
        AddFace(2, 4, 11)
        AddFace(6, 2, 10)
        AddFace(8, 6, 7)
        AddFace(9, 8, 1)

        AddLineIndices(1, 0)
        AddLineIndices(1, 5)
        AddLineIndices(1, 7)
        AddLineIndices(1, 8)
        AddLineIndices(1, 9)

        AddLineIndices(2, 3)
        AddLineIndices(2, 4)
        AddLineIndices(2, 6)
        AddLineIndices(2, 10)
        AddLineIndices(2, 11)

        AddLineIndices(0, 5)
        AddLineIndices(5, 9)
        AddLineIndices(9, 8)
        AddLineIndices(8, 7)
        AddLineIndices(7, 0)

        AddLineIndices(10, 11)
        AddLineIndices(11, 4)
        AddLineIndices(4, 3)
        AddLineIndices(3, 6)
        AddLineIndices(6, 10)

        AddLineIndices(0, 11)
        AddLineIndices(11, 5)
        AddLineIndices(5, 4)
        AddLineIndices(4, 9)
        AddLineIndices(9, 3)
        AddLineIndices(3, 8)
        AddLineIndices(8, 6)
        AddLineIndices(6, 7)
        AddLineIndices(7, 10)
        AddLineIndices(10, 0)

        ' Now we can cycle for the recursion level
        For i As Integer = 0 To recursionLevel - 1
            Dim faces2 As New LinkedList(Of TriangleIndices)()

            Dim j As LinkedList(Of TriangleIndices).Enumerator = faces.GetEnumerator()
            While j.MoveNext()
                Dim f As TriangleIndices = j.Current
                Dim a As Integer = GetMiddlePoint(f.v1, f.v2)
                Dim b As Integer = GetMiddlePoint(f.v2, f.v3)
                Dim c As Integer = GetMiddlePoint(f.v3, f.v1)

                RemoveLineIndices(f.v1, f.v2)
                RemoveLineIndices(f.v2, f.v3)
                RemoveLineIndices(f.v3, f.v1)

                faces2.AddLast(New TriangleIndices(f.v1, a, c))
                faces2.AddLast(New TriangleIndices(f.v2, b, a))
                faces2.AddLast(New TriangleIndices(f.v3, c, b))
                faces2.AddLast(New TriangleIndices(a, b, c))

                AddTriangleLines(f.v1, a, c)
                AddTriangleLines(f.v2, b, a)
                AddTriangleLines(f.v3, c, b)
            End While

            faces = faces2
        Next
    End Sub

    Public Sub AddLineIndices(ByVal index0 As Integer, ByVal index1 As Integer)
        _lineIndices.AddLast(New LineIndices(index0, index1))
    End Sub
    Public Sub RemoveLineIndices(ByVal index0 As Integer, ByVal index1 As Integer)
        Dim result As LinkedListNode(Of LineIndices) = _lineIndices.Find(New LineIndices(index0, index1))
        If result IsNot Nothing Then
            _lineIndices.Remove(result)
        End If

    End Sub
    Public Sub AddTriangleLines(ByVal index0 As Integer, ByVal index1 As Integer, ByVal index2 As Integer)
        AddLineIndices(index0, index1)
        AddLineIndices(index1, index2)
        AddLineIndices(index2, index0)
    End Sub
    Public Function AddVertex(ByVal vertex As Vector3) As Integer
        Dim length As Single = vertex.Length
        vertices.Add(New Vector3(vertex.x / length, vertex.y / length, vertex.z / length))

        index += 1
        Return index - 1
    End Function
    Public Function GetMiddlePoint(ByVal index0 As Integer, ByVal index1 As Integer) As Integer
        Dim isFirstSmaller As Boolean = index0 < index1
        Dim smallerIndex As Long = If(isFirstSmaller, index0, index1)
        Dim largerIndex As Long = If(isFirstSmaller, index1, index0)
        Dim key As Long = (smallerIndex << 32) Or largerIndex

        If middlePointIndexCache.ContainsKey(key) AndAlso middlePointIndexCache(key) <> middlePointIndexCache.Keys.Count Then
            Return middlePointIndexCache(key)
        End If

        Dim point1 As Vector3 = vertices(index0)
        Dim point2 As Vector3 = vertices(index1)
        Dim middle As Vector3 = point1.MidPoint(point2)

        Dim index As Integer = AddVertex(middle)
        If middlePointIndexCache.ContainsKey(key) = False Then
            middlePointIndexCache.Add(key, index)
        Else

            middlePointIndexCache(key) = index
        End If
        Return index
    End Function
    Public Sub AddFace(ByVal index0 As Integer, ByVal index1 As Integer, ByVal index2 As Integer)
        faces.AddLast(New TriangleIndices(index0, index1, index2))
    End Sub
    Public Sub AddToLineIndices(ByVal baseIndex As Integer, ByVal tarGet As LinkedList(Of Integer))
        Dim i As LinkedList(Of LineIndices).Enumerator = _lineIndices.GetEnumerator()
        While i.MoveNext()
            tarGet.AddLast(baseIndex + i.Current.v1)
            tarGet.AddLast(baseIndex + i.Current.v2)
        End While
    End Sub
    Public Sub AddToTriangleIndices(ByVal baseIndex As Integer, ByVal tarGet As LinkedList(Of Integer))
        Dim i As LinkedList(Of TriangleIndices).Enumerator = faces.GetEnumerator()
        While i.MoveNext()
            tarGet.AddLast(baseIndex + i.Current.v1)
            tarGet.AddLast(baseIndex + i.Current.v2)
            tarGet.AddLast(baseIndex + i.Current.v3)
        End While
    End Sub
    Public Function AddToVertices(ByVal tarGet As LinkedList(Of KeyValuePair(Of Vector3, ColourValue)), ByVal position As Vector3, ByVal colour As ColourValue, ByVal scale As Single) As Integer
        Dim transform As Matrix4 = Matrix4.IDENTITY
        transform.SetTrans(position)
        transform.SetScale(New Vector3(scale, scale, scale))

        For i As Integer = 0 To System.Convert.ToInt32(vertices.Count) - 1
            tarGet.AddLast(New KeyValuePair(Of Vector3, ColourValue)(transform * vertices(i), colour))
        Next

        Return vertices.Count
    End Function

End Class
'public class IcoSphere
#End Region


''' <summary>
''' This is the port of the Ogre DebugDrawer from https://bitbucket.org/hasyimi/ogre-debug-drawing-utility/
''' </summary>
''' <remarks></remarks>
Public Class DebugDrawer
    Implements System.IDisposable
#Region "Singleton"
    ''' <summary>
    ''' Data member for locking, instead locking on type itself (to avoid deadlocks).
    ''' </summary>
    Private Shared _syncRoot As Object = New System.Object()

    ''' <summary>
    ''' Data member for storing singleton instance. Volatile type for multithreading support.
    ''' </summary>
    Private Shared _Singleton As DebugDrawer
    ''' <summary>
    ''' The singleton of DebugDrawer. Do not forget to set scenemanager and alpha!
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property Singleton() As DebugDrawer
        Get
            If _Singleton Is Nothing Then
                SyncLock _syncRoot
                    If _Singleton Is Nothing Then
                        _Singleton = New DebugDrawer()
                    End If
                End SyncLock
            End If
            Return _Singleton
        End Get
    End Property
#End Region

    ReadOnly DEFAULT_ICOSPHERE_RECURSION_LEVEL As Integer = 0


#Region "Data Members"

    Private m_sceneManager As SceneManager
    ''' <summary>
    ''' This is the scenemanager which is used to create the manualobject. 
    ''' If you did not call Initialise(...) and you try to call build, an exception will be thrown ;)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property SceneManager() As SceneManager
        Get
            Return m_sceneManager
        End Get
    End Property

    Private m_manualObject As ManualObject
    ''' <summary>
    ''' The manualobject which is used to draw the meshes. You can use this to create a mesh (-snapshot). Remember to call Initialise(...)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ManualObject() As ManualObject
        Get
            Return m_manualObject
        End Get
    End Property

    Private fillAlpha As Single = 0.6

    Private icoSphere As New IcoSphere()
    ' |?| This IcoSphere should be disposed in the Dispose()
    Private isEnabled As Boolean = True
    Public Property Enabled() As Boolean
        Get
            Return isEnabled
        End Get
        Set(ByVal value As Boolean)
            isEnabled = value
        End Set
    End Property
    ''' <summary>
    ''' The alpha value of all faces (from 0 to 1, default 0.6)
    ''' Note: You need to call 'Build' in order to change alpha value of ALL faces. 
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Alpha As Single
        Get
            Return fillAlpha
        End Get
        Set(ByVal value As Single)
            If value > 1 Then
                value = 1
            ElseIf value < 0 Then
                value = 0
            End If
            fillAlpha = value
        End Set
    End Property
    Private isInitialised As Boolean = False
    ' Explicit initialization here is redundant: C# default bool is 'false'
    Public Property Initialized() As Boolean
        Get
            Return isInitialised
        End Get
        Set(ByVal value As Boolean)
            isInitialised = value
        End Set
    End Property

    ' Clear() - Start data members cleared by Clear()
    Private lineVertices As New LinkedList(Of KeyValuePair(Of Vector3, ColourValue))()
    Private triangleVertices As New LinkedList(Of KeyValuePair(Of Vector3, ColourValue))()
    Private lineIndices As New LinkedList(Of Integer)()
    Private triangleIndices As New LinkedList(Of Integer)()

    Private linesIndex As Integer
    Private trianglesIndex As Integer
    ' Clear() - End data members cleared by Clear()

#End Region


    ''' <summary>
    ''' Class Constructor - class is a Singleton (see Design Patterns)
    ''' <remarks>Protected constructor to enable inherited classes to override it, but to avoid
    ''' direct calls from external classes (they must pass through Singleton property)</remarks>
    ''' </summary>
    Protected Sub New()
    End Sub

    Public Sub Initialise(ByVal aSceneManager As SceneManager, ByVal aFillAlpha As Single)
        If isInitialised Then
            ' Initialization multiple call guard
            Return
        End If

        If aSceneManager Is Nothing Then
            Return
        End If

        m_sceneManager = aSceneManager
        fillAlpha = aFillAlpha
        m_manualObject = Nothing
        linesIndex = 0
        trianglesIndex = 0

        m_manualObject = m_sceneManager.CreateManualObject("debugDrawer_object")
        m_manualObject.CastShadows = False

        m_sceneManager.RootSceneNode.CreateChildSceneNode("debugDrawer_object").AttachObject(m_manualObject)
        m_manualObject.Dynamic = (True)

        icoSphere.Create(Me.DEFAULT_ICOSPHERE_RECURSION_LEVEL)

        m_manualObject.Begin("debug_draw", RenderOperation.OperationTypes.OT_LINE_LIST)
        m_manualObject.Position(Vector3.ZERO)
        m_manualObject.Colour(ColourValue.ZERO)
        m_manualObject.Index(0)
        m_manualObject.[End]()
        m_manualObject.Begin("debug_draw", RenderOperation.OperationTypes.OT_TRIANGLE_LIST)
        m_manualObject.Position(Vector3.ZERO)
        m_manualObject.Colour(ColourValue.ZERO)
        m_manualObject.Index(0)
        m_manualObject.[End]()

        trianglesIndex = 0
        linesIndex = trianglesIndex

        isInitialised = True
        ' Initialization multiple call guard
    End Sub
    Public Sub SetIcoSphereRecursionLevel(ByVal recursionLevel As Integer)
        icoSphere.Create(recursionLevel)
    End Sub
    Public Sub Shutdown()
        m_sceneManager.DestroySceneNode("debugDrawer_object")
        m_sceneManager.DestroyManualObject(m_manualObject)
    End Sub
    Public Sub SwitchEnabled()
        isEnabled = Not isEnabled
    End Sub
    Public Sub BuildLine(ByVal start As Vector3, ByVal [end] As Vector3, ByVal colour As ColourValue, ByVal alpha As Single)
        Dim i As Integer = AddLineVertex(start, New ColourValue(colour.r, colour.g, colour.b, alpha))
        AddLineVertex([end], New ColourValue(colour.r, colour.g, colour.b, alpha))

        AddLineIndices(i, i + 1)
    End Sub
    Public Sub BuildQuad(ByVal vertices As Vector3(), ByVal colour As ColourValue, ByVal alpha As Single)
        Dim index As Integer = AddLineVertex(vertices(0), New ColourValue(colour.r, colour.g, colour.b, alpha))
        AddLineVertex(vertices(1), New ColourValue(colour.r, colour.g, colour.b, alpha))
        AddLineVertex(vertices(2), New ColourValue(colour.r, colour.g, colour.b, alpha))
        AddLineVertex(vertices(3), New ColourValue(colour.r, colour.g, colour.b, alpha))

        For i As Integer = 0 To 3
            AddLineIndices(index + i, index + ((i + 1) Mod 4))
        Next
    End Sub
    Public Sub BuildCircle(ByVal centre As Vector3, ByVal radius As Single, ByVal segmentsCount As Integer, ByVal colour As ColourValue, ByVal alpha As Single)
        Dim index As Integer = linesIndex
        Dim increment As Single = 2.0 * Math.PI \ segmentsCount
        Dim angle As Single = 0.0F

        For i As Integer = 0 To segmentsCount - 1
            AddLineVertex(New Vector3(centre.x + radius * Math.Cos(angle), centre.y, centre.z + radius * Math.Sin(angle)), New ColourValue(colour.r, colour.g, colour.b, alpha))
            angle += increment
        Next

        For i As Integer = 0 To segmentsCount - 1
            AddLineIndices(index + i, If(i + 1 < segmentsCount, index + i + 1, index))
        Next
    End Sub
    Public Sub BuildFilledCircle(ByVal centre As Vector3, ByVal radius As Single, ByVal segmentsCount As Integer, ByVal colour As ColourValue, ByVal alpha As Single)
        Dim index As Integer = trianglesIndex
        Dim increment As Single = 2.0 * Math.PI \ segmentsCount
        Dim angle As Single = 0.0F

        For i As Integer = 0 To segmentsCount - 1
            AddTriangleVertex(New Vector3(centre.x + radius * Math.Cos(angle), centre.y, centre.z + radius * Math.Sin(angle)), New ColourValue(colour.r, colour.g, colour.b, alpha))
            angle += increment
        Next

        AddTriangleVertex(centre, New ColourValue(colour.r, colour.g, colour.b, alpha))

        For i As Integer = 0 To segmentsCount - 1
            AddTriangleIndices(If(i + 1 < segmentsCount, index + i + 1, index), index + i, index + segmentsCount)
        Next
    End Sub
    Public Sub BuildCylinder(ByVal centre As Vector3, ByVal radius As Single, ByVal segmentsCount As Integer, ByVal height As Single, ByVal colour As ColourValue, ByVal alpha As Single)
        Dim index As Integer = linesIndex
        Dim increment As Single = System.Convert.ToSingle(2.0 * Math.PI \ segmentsCount)
        Dim angle As Single = 0.0F

        ' Top circle
        For i As Integer = 0 To segmentsCount - 1
            AddLineVertex(New Vector3(System.Convert.ToSingle(centre.x + radius * Math.Cos(angle)), System.Convert.ToSingle(centre.y + height / 2), System.Convert.ToSingle(centre.z + radius * Math.Sin(angle))), New ColourValue(colour.r, colour.g, colour.b, alpha))
            angle += increment
        Next

        angle = 0.0F

        ' Bottom circle
        For i As Integer = 0 To segmentsCount - 1
            AddLineVertex(New Vector3(centre.x + radius * Math.Cos(angle), centre.y - height / 2, centre.z + radius * Math.Sin(angle)), New ColourValue(colour.r, colour.g, colour.b, alpha))
            angle += increment
        Next

        For i As Integer = 0 To segmentsCount - 1
            AddLineIndices(index + i, If(i + 1 < segmentsCount, index + i + 1, index))
            AddLineIndices(segmentsCount + index + i, If(i + 1 < segmentsCount, segmentsCount + index + i + 1, segmentsCount + index))
            AddLineIndices(index + i, segmentsCount + index + i)
        Next
    End Sub
    Public Sub BuildFilledCylinder(ByVal centre As Vector3, ByVal radius As Single, ByVal segmentsCount As Integer, ByVal height As Single, ByVal colour As ColourValue, ByVal alpha As Single)
        Dim index As Integer = trianglesIndex
        Dim increment As Single = 2 * Math.PI \ segmentsCount
        Dim angle As Single = 0.0F

        ' Top circle
        For i As Integer = 0 To segmentsCount - 1
            AddTriangleVertex(New Vector3(centre.x + radius * Math.Cos(angle), centre.y + height / 2, centre.z + radius * Math.Sin(angle)), New ColourValue(colour.r, colour.g, colour.b, alpha))
            angle += increment
        Next

        AddTriangleVertex(New Vector3(centre.x, centre.y + height / 2, centre.z), New ColourValue(colour.r, colour.g, colour.b, alpha))

        angle = 0.0F

        ' Bottom circle
        For i As Integer = 0 To segmentsCount - 1
            AddTriangleVertex(New Vector3(centre.x + radius * Math.Cos(angle), centre.y - height / 2, centre.z + radius * Math.Sin(angle)), New ColourValue(colour.r, colour.g, colour.b, alpha))
            angle += increment
        Next

        AddTriangleVertex(New Vector3(centre.x, centre.y - height / 2, centre.z), New ColourValue(colour.r, colour.g, colour.b, alpha))

        For i As Integer = 0 To segmentsCount - 1
            AddTriangleIndices(If(i + 1 < segmentsCount, index + i + 1, index), index + i, index + segmentsCount)

            AddTriangleIndices(If(i + 1 < segmentsCount, (segmentsCount + 1) + index + i + 1, (segmentsCount + 1) + index), (segmentsCount + 1) + index + segmentsCount, (segmentsCount + 1) + index + i)

            AddQuadIndices(index + i, If(i + 1 < segmentsCount, index + i + 1, index), If(i + 1 < segmentsCount, (segmentsCount + 1) + index + i + 1, (segmentsCount + 1) + index), (segmentsCount + 1) + index + i)
        Next
    End Sub
    Public Sub BuildCuboid(ByVal vertices As Vector3(), ByVal colour As ColourValue, ByVal alpha As Single)
        Dim index As Integer = AddLineVertex(vertices(0), New ColourValue(colour.r, colour.g, colour.b, alpha))
        For i As Integer = 1 To 7
            AddLineVertex(vertices(i), New ColourValue(colour.r, colour.g, colour.b, alpha))
        Next

        For i As Integer = 0 To 3
            AddLineIndices(index + i, index + ((i + 1) Mod 4))
        Next
        For i As Integer = 4 To 7
            AddLineIndices(index + i, If(i = 7, index + 4, index + i + 1))
        Next
        AddLineIndices(index + 1, index + 5)
        AddLineIndices(index + 2, index + 4)
        AddLineIndices(index, index + 6)
        AddLineIndices(index + 3, index + 7)
    End Sub
    Public Sub BuildFilledCuboid(ByVal vertices As Vector3(), ByVal colour As ColourValue, ByVal alpha As Single)
        Dim index As Integer = AddTriangleVertex(vertices(0), New ColourValue(colour.r, colour.g, colour.b, alpha))
        For i As Integer = 1 To 7
            AddTriangleVertex(vertices(i), New ColourValue(colour.r, colour.g, colour.b, alpha))
        Next

        AddQuadIndices(index, index + 1, index + 2, index + 3)
        AddQuadIndices(index + 4, index + 5, index + 6, index + 7)

        AddQuadIndices(index + 1, index + 5, index + 4, index + 2)
        AddQuadIndices(index, index + 3, index + 7, index + 6)

        AddQuadIndices(index + 1, index, index + 6, index + 5)
        AddQuadIndices(index + 4, index + 7, index + 3, index + 2)
    End Sub
    Public Sub BuildFilledQuad(ByVal vertices As Vector3(), ByVal colour As ColourValue, ByVal alpha As Single)
        Dim index As Integer = AddTriangleVertex(vertices(0), New ColourValue(colour.r, colour.g, colour.b, alpha))
        AddTriangleVertex(vertices(1), New ColourValue(colour.r, colour.g, colour.b, alpha))
        AddTriangleVertex(vertices(2), New ColourValue(colour.r, colour.g, colour.b, alpha))
        AddTriangleVertex(vertices(3), New ColourValue(colour.r, colour.g, colour.b, alpha))

        AddQuadIndices(index, index + 1, index + 2, index + 3)
    End Sub
    Public Sub BuildFilledTriangle(ByVal vertices As Vector3(), ByVal colour As ColourValue, ByVal alpha As Single)
        Dim index As Integer = AddTriangleVertex(vertices(0), New ColourValue(colour.r, colour.g, colour.b, alpha))
        AddTriangleVertex(vertices(1), New ColourValue(colour.r, colour.g, colour.b, alpha))
        AddTriangleVertex(vertices(2), New ColourValue(colour.r, colour.g, colour.b, alpha))

        AddTriangleIndices(index, index + 1, index + 2)
    End Sub
    Public Sub BuildTetrahedron(ByVal centre As Vector3, ByVal scale As Single, ByVal colour As ColourValue, ByVal alpha As Single)
        Dim index As Integer = linesIndex

        ' Distance from the centre
        Dim bottomDistance As Single = scale * 0.2F
        Dim topDistance As Single = scale * 0.62F
        Dim frontDistance As Single = scale * 0.289F
        Dim backDistance As Single = scale * 0.577F
        Dim leftRightDistance As Single = scale * 0.5F

        AddLineVertex(New Vector3(centre.x, centre.y + topDistance, centre.z), New ColourValue(colour.r, colour.g, colour.b, alpha))
        AddLineVertex(New Vector3(centre.x, centre.y - bottomDistance, centre.z + frontDistance), New ColourValue(colour.r, colour.g, colour.b, alpha))
        AddLineVertex(New Vector3(centre.x + leftRightDistance, centre.y - bottomDistance, centre.z - backDistance), New ColourValue(colour.r, colour.g, colour.b, alpha))
        AddLineVertex(New Vector3(centre.x - leftRightDistance, centre.y - bottomDistance, centre.z - backDistance), New ColourValue(colour.r, colour.g, colour.b, alpha))

        AddLineIndices(index, index + 1)
        AddLineIndices(index, index + 2)
        AddLineIndices(index, index + 3)

        AddLineIndices(index + 1, index + 2)
        AddLineIndices(index + 2, index + 3)
        AddLineIndices(index + 3, index + 1)
    End Sub
    Public Sub BuildFilledTetrahedron(ByVal centre As Vector3, ByVal scale As Single, ByVal colour As ColourValue, ByVal alpha As Single)
        Dim index As Integer = trianglesIndex

        ' Distance from the centre
        Dim bottomDistance As Single = scale * 0.2F
        Dim topDistance As Single = scale * 0.62F
        Dim frontDistance As Single = scale * 0.289F
        Dim backDistance As Single = scale * 0.577F
        Dim leftRightDistance As Single = scale * 0.5F

        AddTriangleVertex(New Vector3(centre.x, centre.y + topDistance, centre.z), New ColourValue(colour.r, colour.g, colour.b, alpha))
        AddTriangleVertex(New Vector3(centre.x, centre.y - bottomDistance, centre.z + frontDistance), New ColourValue(colour.r, colour.g, colour.b, alpha))
        AddTriangleVertex(New Vector3(centre.x + leftRightDistance, centre.y - bottomDistance, centre.z - backDistance), New ColourValue(colour.r, colour.g, colour.b, alpha))
        AddTriangleVertex(New Vector3(centre.x - leftRightDistance, centre.y - bottomDistance, centre.z - backDistance), New ColourValue(colour.r, colour.g, colour.b, alpha))

        AddTriangleIndices(index, index + 1, index + 2)
        AddTriangleIndices(index, index + 2, index + 3)
        AddTriangleIndices(index, index + 3, index + 1)

        AddTriangleIndices(index + 1, index + 3, index + 2)
    End Sub
    Public Sub DrawLine(ByVal start As Vector3, ByVal [end] As Vector3, ByVal colour As ColourValue)
        BuildLine(start, [end], colour, 1)
    End Sub
    Public Sub DrawCircle(ByVal centre As Vector3, ByVal radius As Single, ByVal segmentsCount As Integer, ByVal colour As ColourValue, ByVal isFilled As Boolean)
        BuildCircle(centre, radius, segmentsCount, colour, 1)
        If isFilled Then
            BuildFilledCircle(centre, radius, segmentsCount, colour, fillAlpha)
        End If
    End Sub
    Public Sub DrawCylinder(ByVal centre As Vector3, ByVal radius As Single, ByVal segmentsCount As Integer, ByVal height As Single, ByVal colour As ColourValue, ByVal isFilled As Boolean)
        BuildCylinder(centre, radius, segmentsCount, height, colour, 1)
        If isFilled Then
            BuildFilledCylinder(centre, radius, segmentsCount, height, colour, fillAlpha)
        End If
    End Sub
    Public Sub DrawQuad(ByVal vertices As Vector3(), ByVal colour As ColourValue, ByVal isFilled As Boolean)
        BuildQuad(vertices, colour, 1)
        If isFilled Then
            BuildFilledQuad(vertices, colour, fillAlpha)
        End If
    End Sub
    Public Sub DrawCuboid(ByVal vertices As Vector3(), ByVal colour As ColourValue, ByVal isFilled As Boolean)
        BuildCuboid(vertices, colour, 1)
        If isFilled Then
            BuildFilledCuboid(vertices, colour, fillAlpha)
        End If
    End Sub
    Public Sub DrawSphere(ByVal centre As Vector3, ByVal radius As Single, ByVal colour As ColourValue, ByVal isFilled As Boolean)
        Dim baseIndex As Integer = linesIndex
        linesIndex += icoSphere.AddToVertices(lineVertices, centre, colour, radius)
        icoSphere.AddToLineIndices(baseIndex, lineIndices)

        If isFilled Then
            baseIndex = trianglesIndex
            trianglesIndex += icoSphere.AddToVertices(triangleVertices, centre, New ColourValue(colour.r, colour.g, colour.b, fillAlpha), radius)
            icoSphere.AddToTriangleIndices(baseIndex, triangleIndices)
        End If
    End Sub
    Public Sub DrawTetrahedron(ByVal centre As Vector3, ByVal scale As Single, ByVal colour As ColourValue, ByVal isFilled As Boolean)
        BuildTetrahedron(centre, scale, colour, 1)
        If isFilled Then
            BuildFilledTetrahedron(centre, scale, colour, fillAlpha)
        End If
    End Sub
    Public Sub Build()
        If Initialized = False Then
            Throw New Exception("You forgot to call 'Initialise(...)'")
        End If
        m_manualObject.BeginUpdate(0)
        If lineVertices.Count > 0 AndAlso isEnabled Then
            m_manualObject.EstimateVertexCount(System.Convert.ToUInt32(lineVertices.Count))
            m_manualObject.EstimateIndexCount(System.Convert.ToUInt32(lineIndices.Count))
            Dim i As LinkedList(Of KeyValuePair(Of Vector3, ColourValue)).Enumerator = lineVertices.GetEnumerator()
            While i.MoveNext()
                m_manualObject.Position(i.Current.Key)
                m_manualObject.Colour(i.Current.Value)
            End While
            Dim i2 As LinkedList(Of Integer).Enumerator = lineIndices.GetEnumerator()
            While i2.MoveNext()
                m_manualObject.Index(System.Convert.ToUInt16(i2.Current))
            End While
        End If
        m_manualObject.[End]()

        m_manualObject.BeginUpdate(1)
        If triangleVertices.Count > 0 AndAlso isEnabled Then
            m_manualObject.EstimateVertexCount(System.Convert.ToUInt16((triangleVertices.Count)))
            m_manualObject.EstimateIndexCount(System.Convert.ToUInt16(triangleIndices.Count))
            Dim i As LinkedList(Of KeyValuePair(Of Vector3, ColourValue)).Enumerator = triangleVertices.GetEnumerator()
            While i.MoveNext()
                m_manualObject.Position(i.Current.Key)
                m_manualObject.Colour(i.Current.Value.r, i.Current.Value.g, i.Current.Value.b, fillAlpha)
            End While
            Dim i2 As LinkedList(Of Integer).Enumerator = triangleIndices.GetEnumerator()
            While i2.MoveNext()
                m_manualObject.Index(System.Convert.ToUInt16(i2.Current))
            End While
        End If
        m_manualObject.[End]()
    End Sub
    Public Sub Clear()
        lineVertices.Clear()
        triangleVertices.Clear()
        lineIndices.Clear()
        triangleIndices.Clear()
        trianglesIndex = 0
        linesIndex = trianglesIndex
    End Sub
    Public Function AddLineVertex(ByVal vertex As Vector3, ByVal colour As ColourValue) As Integer
        lineVertices.AddLast(New KeyValuePair(Of Vector3, ColourValue)(vertex, colour))

        linesIndex += 1
        Return linesIndex - 1
    End Function
    Public Sub AddLineIndices(ByVal index1 As Integer, ByVal index2 As Integer)
        lineIndices.AddLast(index1)
        lineIndices.AddLast(index2)
    End Sub
    Public Function AddTriangleVertex(ByVal vertex As Vector3, ByVal colour As ColourValue) As Integer
        triangleVertices.AddLast(New KeyValuePair(Of Vector3, ColourValue)(vertex, colour))

        trianglesIndex += 1
        Return trianglesIndex - 1
    End Function
    Public Sub AddTriangleIndices(ByVal index1 As Integer, ByVal index2 As Integer, ByVal index3 As Integer)
        triangleIndices.AddLast(index1)
        triangleIndices.AddLast(index2)
        triangleIndices.AddLast(index3)
    End Sub
    Public Sub AddQuadIndices(ByVal index1 As Integer, ByVal index2 As Integer, ByVal index3 As Integer, ByVal index4 As Integer)
        triangleIndices.AddLast(index1)
        triangleIndices.AddLast(index2)
        triangleIndices.AddLast(index3)

        triangleIndices.AddLast(index1)
        triangleIndices.AddLast(index3)
        triangleIndices.AddLast(index4)
    End Sub


#Region "IDisposable Support"
    ' To identify redundant calls
    Private disposedValue As Boolean
    ' default bool is 'false'
    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            ' TODO: Verwalteten Zustand löschen (verwaltete Objekte).

            ' TODO: Test these two!
            'icoSphere.Dispose();
            'Clear();
            If disposing Then
            End If
            ' TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalize() unten überschreiben.
            ' TODO: Große Felder auf NULL festlegen.
            Shutdown()
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: Finalize() nur überschreiben, wenn Dispose(ByVal disposing As Boolean) oben über Code zum Freigeben von nicht verwalteten Ressourcen verfügt.
    'Protected Overrides Sub Finalize()
    '    ' Ändern Sie diesen Code nicht. Fügen Sie oben in Dispose(ByVal disposing As Boolean) Bereinigungscode ein.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code is for C# to implement the Dispose pattern correctly.
    Public Sub Dispose() Implements System.IDisposable.Dispose
        Dispose(True)
        System.GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class