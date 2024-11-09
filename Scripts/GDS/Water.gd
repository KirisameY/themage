@tool
extends MeshInstance3D
func _physics_process(_delta: float) -> void:
	global_position = Vector3(int(%Player.global_position.x),global_position.y,int(%Player.global_position.z))
